namespace MvcCoreSample.Models;

public class ModifyParcelsFilterViewModel
{
    public bool RefreshMap { get; set; }

    public string? ModifiedFilter { get; set; }

    public CommonInvokeUrlRequestModel? RequestParams { get; set; }
}
