namespace MvcCoreSample.Models;

public class ListSelectionRequestModel : CommonInvokeUrlRequestModel
{
    public string SelectionXml { get; set; } = null!;
}
