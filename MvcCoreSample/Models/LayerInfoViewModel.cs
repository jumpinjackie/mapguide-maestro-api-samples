using Microsoft.AspNetCore.Mvc.Rendering;
using OSGeo.MapGuide.MaestroAPI.Mapping;

namespace MvcCoreSample.Models;

public class LayerInfoViewModel
{
    public LayerInfoViewModel(RuntimeMap rtMap)
    {
        this.Layers = new List<SelectListItem>();
        foreach (RuntimeMapLayer rtLayer in rtMap.Layers)
        {
            this.Layers.Add(new SelectListItem(rtLayer.Name, rtLayer.ObjectId));
        }
    }

    public List<SelectListItem> Layers { get; private set; } = null!;
}
