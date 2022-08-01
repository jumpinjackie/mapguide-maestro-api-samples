namespace MvcCoreSample.Models;

public class CommonResponseModel
{
    protected CommonResponseModel(CommonInvokeUrlRequestModel model)
    {
        this.Session = model.Session;
        this.MapName = model.MapName;
    }

    public string MapName { get; private set; }

    public string Session { get; private set; }
}
