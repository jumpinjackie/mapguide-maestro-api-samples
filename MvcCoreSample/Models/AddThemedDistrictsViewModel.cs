namespace MvcCoreSample.Models;

public class AddThemedDistrictsViewModel : CommonResponseModel
{
    public AddThemedDistrictsViewModel(CommonInvokeUrlRequestModel model)
        : base(model)
    { }

    public string? Error { get; set; }

    public string? Message { get; set; }

    public bool RefreshMap { get; set; }
}
