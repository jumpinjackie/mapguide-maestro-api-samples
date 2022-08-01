namespace MvcCoreSample.Models;

public class ModifyParcelsFilterViewModel : CommonResponseModel
{
    public ModifyParcelsFilterViewModel(CommonInvokeUrlRequestModel model)
        : base(model)
    { }

    public bool RefreshMap { get; set; }

    public string? ModifiedFilter { get; set; }

    public CommonInvokeUrlRequestModel? RequestParams { get; set; }
}
