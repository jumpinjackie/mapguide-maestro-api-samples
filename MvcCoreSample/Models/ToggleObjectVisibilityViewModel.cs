namespace MvcCoreSample.Models;

public class ToggleObjectVisibilityViewModel : CommonResponseModel
{
    public ToggleObjectVisibilityViewModel(CommonInvokeUrlRequestModel model)
        : base(model)
    {
    }

    public string? Error { get; set; }

    public string? Message { get; set; }

    public bool RefreshMap { get; set; }
}
