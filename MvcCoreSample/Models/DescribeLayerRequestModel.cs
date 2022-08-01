namespace MvcCoreSample.Models;

public class DescribeLayerRequestModel : CommonInvokeUrlRequestModel
{
    public string LayerObjectId { get; set; } = null!;
}
