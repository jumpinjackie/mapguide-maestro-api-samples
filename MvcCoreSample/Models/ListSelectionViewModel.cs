using OSGeo.MapGuide.MaestroAPI.Mapping;

namespace MvcCoreSample.Models;

public class ListSelectionViewModel
{
    public ListSelectionViewModel(MapSelection selection)
    {
        foreach (var layerSel in selection)
        {
            this.Layers.Add(new LayerSelectionModel(layerSel));
        }
    }

    public List<LayerSelectionModel> Layers { get; } = new List<LayerSelectionModel>();
}

public class LayerSelectionModel
{
    public LayerSelectionModel(MapSelection.LayerSelection layerSel)
    {
        this.LayerName = layerSel.Layer.Name;
        this.LayerObjectId = layerSel.Layer.ObjectId;

        foreach (var featureIdValues in layerSel)
        {
            this.Features.Add(new SelectedFeatureModel(featureIdValues, layerSel));
        }
    }

    public string LayerName { get; set; } = null!;

    public string LayerObjectId { get; set; } = null!;

    public List<SelectedFeatureModel> Features { get; } = new List<SelectedFeatureModel>();
}

public class SelectedFeatureModel
{
    public SelectedFeatureModel(object[] featureIdValues, MapSelection.LayerSelection layerSel)
    {
        this.FeatureIdValues = featureIdValues;
        this.FeatureId = layerSel.EncodeIDString(featureIdValues);
    }

    public object[] FeatureIdValues { get; private set; }

    public string FeatureId { get; set; } = null!;
}