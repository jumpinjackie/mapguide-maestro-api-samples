namespace MvcCoreSample.Models;

public enum ParcelLayerAction
{
    Added,
    Removed
}

public class ToggleParcelsLayerViewModel
{
    public ParcelLayerAction Action { get; set; }

    public bool RefreshMap { get; set; }
}
