namespace MvcCoreSample.Models;

public class SetSelectedFeaturesViewModel
{
    public int SelectionCount { get; set; }

    public string LayerName { get; set; } = null!;

    public string SelectionXml { get; set; } = null!;
}
