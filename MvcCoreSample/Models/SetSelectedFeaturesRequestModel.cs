namespace MvcCoreSample.Models;

public class SetSelectedFeaturesRequestModel : CommonInvokeUrlRequestModel
{
    public string LayerObjectId { get; set; } = null!;

    public string Filter { get; set; } = null!;
}
