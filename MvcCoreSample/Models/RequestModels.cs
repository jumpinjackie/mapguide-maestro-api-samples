namespace MvcCoreSample.Models;

public class CommonInvokeUrlRequestModel
{
    public string MapName { get; set; } = null!;

    public string Session { get; set; } = null!;
}

public class FeatureInfoRequestModel : CommonInvokeUrlRequestModel
{
    public string LayerId { get; set; } = null!;

    public string FeatureId { get; set; } = null!;
}