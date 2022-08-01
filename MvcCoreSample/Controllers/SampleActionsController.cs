using Microsoft.AspNetCore.Mvc;
using MvcCoreSample.Models;
using OSGeo.MapGuide.MaestroAPI.Mapping;
using OSGeo.MapGuide.MaestroAPI.Services;
using OSGeo.MapGuide.ObjectModels.LayerDefinition;
using System.Net;
using System.Text;

namespace MvcCoreSample.Controllers;

public class SampleActionsController : Controller
{
    readonly MgConnectionSessionFactory _connFactory;

    public SampleActionsController(MgConnectionSessionFactory connFactory)
    {
        ArgumentNullException.ThrowIfNull(connFactory);
        _connFactory = connFactory;
    }

    [HttpGet]
    public IActionResult ToggleLayerVisibility(ToggleObjectVisibilityRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var conn = _connFactory(model.Session);
        var rtMap = SamplesHelper.OpenMap(conn, model);

        var layer = rtMap.Layers[model.Name];
        if (layer != null)
        {
            layer.Visible = !layer.Visible;

            rtMap.Save(); //Always save changes after modifying

            return View(new ToggleObjectVisibilityViewModel(model) { Message = "Layer (" + layer.Name + ") visible: " + layer.Visible, RefreshMap = true });
        }
        else
        {
            return View(new ToggleObjectVisibilityViewModel(model) { Error = "Layer (" + model.Name + ") not found!" });
        }
    }

    [HttpGet]
    public IActionResult ToggleGroupVisibility(ToggleObjectVisibilityRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var conn = _connFactory(model.Session);
        var rtMap = SamplesHelper.OpenMap(conn, model);
        var group = rtMap.Groups[model.Name];
        if (group != null)
        {
            group.Visible = !group.Visible;

            rtMap.Save(); //Always save changes after modifying

            return View(new ToggleObjectVisibilityViewModel(model) { Message = "Group (" + group.Name + ") visible: " + group.Visible, RefreshMap = true });
        }
        else
        {
            return View(new ToggleObjectVisibilityViewModel(model) { Error = "Group (" + model.Name + ") not found!" });
        }
    }

    [HttpPost]
    public IActionResult SetSelectedFeatures(SetSelectedFeaturesRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var conn = _connFactory(model.Session);
        var rtMap = SamplesHelper.OpenMap(conn, model);

        if (model.LayerObjectId != null && model.Filter != null)
        {

            //Get the selected layer
            var rtLayer = rtMap.Layers.GetByObjectId(model.LayerObjectId);

            //Query using the user filter
            var reader = conn.FeatureService.QueryFeatureSource(
                                        rtLayer.FeatureSourceID,
                                        rtLayer.QualifiedClassName,
                                        model.Filter);

            //Get the selection set
            var sel = new MapSelection(rtMap);
            MapSelection.LayerSelection layerSel;
            if (!sel.Contains(rtLayer))
            {
                sel.Add(rtLayer);
            }
            layerSel = sel[rtLayer];

            //Clear any existing selections
            layerSel.Clear();

            //Populate selection set with query result
            int added = layerSel.AddFeatures(reader, -1);

            //Generate selection string
            string selXml = sel.ToXml();

            return View(new SetSelectedFeaturesViewModel(model, rtMap) { Filter = model.Filter, LayerObjectId = model.LayerObjectId, SelectionCount = added, LayerName = rtLayer.Name, SelectionXml = selXml });
        }
        else
        {
            return View(new SetSelectedFeaturesViewModel(model, rtMap) { });
        }
    }

    [HttpGet]
    public IActionResult ModifyParcelsFilter(ModifyParcelsFilterRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var conn = _connFactory(model.Session);
        var rtMap = SamplesHelper.OpenMap(conn, model);

        int layerIndex = rtMap.Layers.IndexOf("Parcels");
        var layer = rtMap.Layers[layerIndex];

        //Here is now the layer replacement technique works:
        //
        //We take the Layer Definition content referenced by the old layer
        //Modify the filter in this content and save it to a new resource id
        //We then create a replacement layer that points to this new resource id
        //and set the public properties to be identical of the old layer.
        //
        //Finally we then remove the old layer and put the replacement layer in its
        //place, before saving the runtime map.

        var ldf = (ILayerDefinition)conn.ResourceService.GetResource(layer.LayerDefinitionID);
        var vl = (IVectorLayerDefinition)ldf.SubLayer;

        //Sets the layer filter
        vl.Filter = "RNAME LIKE 'SCHMITT%'";
        if (model.Reset)
        {
            vl.Filter = "";
        }

        //Save this modified layer under a different resource id
        string ldfId = "Session:" + conn.SessionID + "//ParcelsFiltered.LayerDefinition";
        conn.ResourceService.SaveResourceAs(ldf, ldfId);
        //Note that SaveResourceAs does not modify the ResourceID of the resource we want to save
        //so we need to update it here
        ldf.ResourceID = ldfId;

        //Create our replacement layer and apply the same properties from the old one
        var mpSvc = (IMappingService)conn.GetService((int)ServiceType.Mapping);
        var replace = mpSvc.CreateMapLayer(rtMap, ldf);
        replace.ExpandInLegend = layer.ExpandInLegend;
        replace.Group = layer.Group;
        replace.LegendLabel = layer.LegendLabel;
        replace.Name = layer.Name;
        replace.Selectable = layer.Selectable;
        replace.ShowInLegend = layer.ShowInLegend;
        replace.Visible = layer.Visible;

        //Remove the old layer and put the new layer at the same position (thus having the
        //same draw order)
        rtMap.Layers.RemoveAt(layerIndex);
        rtMap.Layers.Insert(layerIndex, replace);
        replace.ForceRefresh();

        rtMap.Save();

        if (model.Reset)
        {
            return View(new ModifyParcelsFilterViewModel(model)
            {
                RefreshMap = true
            });
        }
        else
        {
            return View(new ModifyParcelsFilterViewModel(model)
            {
                RefreshMap = true,
                ModifiedFilter = vl.Filter,
                RequestParams = model
            });
        }
    }

    [HttpPost]
    public IActionResult ListSelection(ListSelectionRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var conn = _connFactory(model.Session);
        var rtMap = SamplesHelper.OpenMap(conn, model);
        var selection = new MapSelection(rtMap, model.Selection);
        return View(new ListSelectionViewModel(selection, model));
    }

    [HttpPost]
    public IActionResult DescribeLayer(DescribeLayerRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var conn = _connFactory(model.Session);
        var rtMap = SamplesHelper.OpenMap(conn, model);
        
        //Get the selected layer
        var rtLayer = rtMap.Layers.GetByObjectId(model.LayerObjectId);

        //Get the class definition
        var clsDef = conn.FeatureService.GetClassDefinition(rtLayer.FeatureSourceID, rtLayer.QualifiedClassName);

        return View(new DescribeLayerViewModel(rtLayer, clsDef));
    }

    [HttpGet]
    public IActionResult FeatureInfo(FeatureInfoRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var conn = _connFactory(model.Session);
        var rtMap = SamplesHelper.OpenMap(conn, model);

        var layer = rtMap.Layers.GetByObjectId(model.LayerId);

        //The values returned are in the same order as the array from the IdentityProperties
        var values = layer.ParseSelectionValues(model.FeatureId);
        var idProps = layer.IdentityProperties;

        //Having decoded the identity property values and knowing what names they are from the
        //RuntimeMapLayer, construct the selection filter based on these values.
        //
        //This sample assumes the Sheboygan dataset and so all identity property values are 
        //known to be only numeric or strings. If this is not the case for you, use the Type
        //property in PropertyInfo to determine how to construct the filter
        string[] conditions = new string[idProps.Length];
        for (int i = 0; i < idProps.Length; i++)
        {
            conditions[i] = idProps[i].Name + " = " + values[i].ToString();
        }
        //OR all the conditions together to form our final filter
        string selFilter = string.Join(" OR ", conditions);

        //Execute the query
        var reader = conn.FeatureService.QueryFeatureSource(layer.FeatureSourceID,
                                                            layer.QualifiedClassName,
                                                            selFilter);


        return View(new FeatureInfoViewModel(reader, model));
    }   
}
