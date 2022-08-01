namespace MvcCoreSample.Models;

public class AddTracksViewModel : CommonResponseModel
{
    public AddTracksViewModel(CommonInvokeUrlRequestModel model)
        : base(model)
    { }

    public string? Error { get; set; }

    public string? Message { get; set; }

    public bool RefreshMap { get; set; }
}
