using Microsoft.AspNetCore.Mvc.Rendering;
using OSGeo.MapGuide.MaestroAPI.Mapping;

namespace MvcCoreSample.Models;

public class SetSelectedFeaturesRequestModel : CommonInvokeUrlRequestModel
{
    public SetSelectedFeaturesRequestModel() { }

    public SetSelectedFeaturesRequestModel(CommonInvokeUrlRequestModel model)
    {
        this.Session = model.Session;
        this.MapName = model.MapName;
    }

    public string LayerObjectId { get; set; } = null!;

    public string Filter { get; set; } = null!;
}

public class SetSelectedFeaturesViewModel : CommonResponseModel
{
    public SetSelectedFeaturesViewModel(CommonInvokeUrlRequestModel model, RuntimeMap rtMap)
        : base(model)
    {
        this.AvailableLayers = new List<SelectListItem>();
        foreach (RuntimeMapLayer rtLayer in rtMap.Layers)
        {
            this.AvailableLayers.Add(new SelectListItem(rtLayer.Name, rtLayer.ObjectId));
        }
    }

    public int SelectionCount { get; set; }

    public string LayerName { get; set; } = null!;

    public string SelectionXml { get; set; } = null!;

    public string LayerObjectId { get; set; } = null!;

    public string Filter { get; set; } = null!;

    public List<SelectListItem> AvailableLayers { get; private set; } = null!;
}