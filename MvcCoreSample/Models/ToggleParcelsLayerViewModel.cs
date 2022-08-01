namespace MvcCoreSample.Models;

public enum ParcelLayerAction
{
    Added,
    Removed
}

public class ToggleParcelsLayerViewModel : CommonResponseModel
{
    public ToggleParcelsLayerViewModel(CommonInvokeUrlRequestModel model)
        : base(model)
    { }

    public ParcelLayerAction Action { get; set; }

    public bool RefreshMap { get; set; }
}
