using Microsoft.AspNetCore.Mvc;
using MvcCoreSample.Models;
using OSGeo.MapGuide.MaestroAPI.Services;
using OSGeo.MapGuide.ObjectModels.LayerDefinition;

namespace MvcCoreSample.Controllers;

/// <summary>
/// Every route in this controller represents a URL that only takes session/mapname parameters.
/// 
/// You can set up an InvokeURL command to call into any route here if you like
/// </summary>
public class SampleTasksController : Controller
{
    readonly MgConnectionSessionFactory _connFactory;

    public SampleTasksController(MgConnectionSessionFactory connFactory)
    {
        ArgumentNullException.ThrowIfNull(connFactory);
        _connFactory = connFactory;
    }

    [HttpGet]
    public IActionResult ToggleParcelsLayer(CommonInvokeUrlRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var conn = _connFactory(model.Session);
        var rtMap = SamplesHelper.OpenMap(conn, model);

        var parcels = rtMap.Layers["Parcels"];
        if (parcels != null)
        {
            rtMap.Layers.Remove(parcels);

            rtMap.Save();

            return View(new ToggleParcelsLayerViewModel(model) { RefreshMap = true, Action = ParcelLayerAction.Removed });
        }
        else
        {
            var mpSvc = (IMappingService)conn.GetService((int)ServiceType.Mapping);

            string groupName = "Municipal";
            var group = rtMap.Groups[groupName];
            if (group == null)
            {
                group = mpSvc.CreateMapGroup(rtMap, groupName);
                rtMap.Groups.Add(group);
                throw new Exception("Layer group not found");
            }

            ILayerDefinition layerDef = (ILayerDefinition)conn.ResourceService.GetResource("Library://Samples/Sheboygan/Layers/Parcels.LayerDefinition");
            var layer = mpSvc.CreateMapLayer(rtMap, layerDef);

            layer.Group = group.Name;
            layer.LegendLabel = "Parcels";
            layer.ShowInLegend = true;
            layer.ExpandInLegend = true;
            layer.Selectable = true;
            layer.Visible = true;

            //Set it to be drawn above islands.
            //In terms of draw order, it goes [0...n] -> [TopMost ... Bottom]
            //So for a layer to be drawn above something else, its draw order must be
            //less than that particular layer.

            int index = rtMap.Layers.IndexOf("Islands");
            rtMap.Layers.Insert(index, layer);

            rtMap.Save();

            return View(new ToggleParcelsLayerViewModel(model) { RefreshMap = true, Action = ParcelLayerAction.Added });
        }
    }

    /*
    [HttpGet]
    public IActionResult SetSelectedFeatures(CommonInvokeUrlRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var conn = _connFactory(model.Session);
        var rtMap = SamplesHelper.OpenMap(conn, model);

        return View(new LayerInfoViewModel(rtMap));
    }
    */

    [HttpGet]
    public IActionResult LayerInfo(CommonInvokeUrlRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var conn = _connFactory(model.Session);
        var rtMap = SamplesHelper.OpenMap(conn, model);
        return View(new LayerInfoViewModel(rtMap, model));
    }

    [HttpGet]
    public IActionResult AddTracksLayer(CommonInvokeUrlRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var conn = _connFactory(model.Session);
        var rtMap = SamplesHelper.OpenMap(conn, model);
        var tracks = rtMap.Layers["Tracks"];
        if (tracks != null)
        {
            return View(new AddTracksViewModel(model)
            {
                Error = "Tracks layer already added"
            });
        }
        else
        {
            var mpSvc = (IMappingService)conn.GetService((int)ServiceType.Mapping);
            string groupName = "Transportation";
            var group = rtMap.Groups[groupName];
            if (group == null)
            {
                group = mpSvc.CreateMapGroup(rtMap, groupName);
                rtMap.Groups.Add(group);
            }

            //For some reason, the Sheboygan sample data does not have a Rail
            //Layer Definition, so what better time to show how to create a
            //layer dynamically :)

            //Our Feature Source
            string fsId = "Library://Samples/Sheboygan/Data/Rail.FeatureSource";

            //The place we'll store the layer definition
            string layerId = "Session:" + conn.SessionID + "//Rail.LayerDefinition";

            SamplesHelper.CreateTracksLayer(conn, fsId, layerId);

            var layerDef = (ILayerDefinition)conn.ResourceService.GetResource(layerId);
            var layer = mpSvc.CreateMapLayer(rtMap, layerDef);

            layer.Group = groupName;
            layer.Name = "Tracks";
            layer.LegendLabel = "Tracks";
            layer.ShowInLegend = true;
            layer.ExpandInLegend = true;
            layer.Selectable = true;
            layer.Visible = true;

            //Set it to be drawn above districts.
            //In terms of draw order, it goes [0...n] -> [TopMost ... Bottom]
            //So for a layer to be drawn above something else, its draw order must be
            //less than that particular layer.

            int index = rtMap.Layers.IndexOf("Districts");
            rtMap.Layers.Insert(index, layer);

            rtMap.Save();

            return View(new AddTracksViewModel(model)
            {
                Message = "Tracks layer added",
                RefreshMap = true
            });
        }
    }

    [HttpGet]
    public IActionResult AddThemedDistrictsLayer(CommonInvokeUrlRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var conn = _connFactory(model.Session);
        var rtMap = SamplesHelper.OpenMap(conn, model);
        var districts = rtMap.Layers["ThemedDistricts"];

        if (districts != null)
        {
            return View(new AddThemedDistrictsViewModel(model)
            {
                Error = "Themed districts layer already added"
            });
        }
        else
        {
            //Add our themed districts layer

            //Our Feature Source
            string fsId = "Library://Samples/Sheboygan/Data/VotingDistricts.FeatureSource";

            //The place we'll store the layer definition
            string layerId = "Session:" + conn.SessionID + "//ThemedVotingDistricts.LayerDefinition";

            SamplesHelper.CreateDistrictsLayer(conn, fsId, layerId);

            var layerDef = (ILayerDefinition)conn.ResourceService.GetResource(layerId);
            var mpSvc = (IMappingService)conn.GetService((int)ServiceType.Mapping);
            var layer = mpSvc.CreateMapLayer(rtMap, layerDef);

            layer.Name = "ThemedDistricts";
            layer.Group = "";
            layer.LegendLabel = "Themed Districts";
            layer.ShowInLegend = true;
            layer.ExpandInLegend = true;
            layer.Selectable = true;
            layer.Visible = true;

            //Set it to be drawn above districts.
            //In terms of draw order, it goes [0...n] -> [TopMost ... Bottom]
            //So for a layer to be drawn above something else, its draw order must be
            //less than that particular layer.

            int index = rtMap.Layers.IndexOf("Districts");
            rtMap.Layers.Insert(index, layer);

            rtMap.Save();

            return View(new AddThemedDistrictsViewModel(model)
            {
                Message = "Themed districts layer added",
                RefreshMap = true
            });
        }
    }
}
