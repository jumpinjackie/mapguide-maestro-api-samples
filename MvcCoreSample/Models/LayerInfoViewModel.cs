using Microsoft.AspNetCore.Mvc.Rendering;
using OSGeo.MapGuide.MaestroAPI.Mapping;

namespace MvcCoreSample.Models;

public class LayerInfoViewModel : CommonResponseModel
{
    public LayerInfoViewModel(RuntimeMap rtMap, CommonInvokeUrlRequestModel model)
        : base(model)
    {
        this.AvailableLayers = new List<SelectListItem>();
        foreach (RuntimeMapLayer rtLayer in rtMap.Layers)
        {
            this.AvailableLayers.Add(new SelectListItem(rtLayer.Name, rtLayer.ObjectId));
        }
    }

    public List<SelectListItem> AvailableLayers { get; private set; } = null!;
}
