namespace MvcCoreSample.Models;

public class ListSelectionRequestModel : CommonInvokeUrlRequestModel
{
    /// <summary>
    /// The active selection set XML
    /// </summary>
    public string Selection { get; set; } = null!;
}
