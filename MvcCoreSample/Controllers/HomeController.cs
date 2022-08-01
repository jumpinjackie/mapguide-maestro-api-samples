using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcCoreSample.Models;
using OSGeo.MapGuide.ObjectModels;
using OSGeo.MapGuide.ObjectModels.WebLayout;

namespace MvcCoreSample.Controllers;

public class HomeController : Controller
{
    readonly ILogger<HomeController> _logger;
    readonly MgConnectionLoginFactory _connFactory;
    readonly IConfiguration _conf;

    public HomeController(ILogger<HomeController> logger, MgConnectionLoginFactory connFactory, IConfiguration conf)
    {
        _logger = logger;
        _connFactory = connFactory;
        _conf = conf;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult TaskPane(CommonInvokeUrlRequestModel model) => View(model);

    public IActionResult Viewer()
    {
        var conn = _connFactory("Anonymous", "");

        var mdfId = "Library://Samples/Sheboygan/Maps/Sheboygan.MapDefinition";
        if (conn.ResourceService.ResourceExists(mdfId))
        {
            //Here's an example of pre-processing the WebLayout before loading it
            //in the AJAX viewer.

            //This technique can also be used to do things like:
            //
            // 1. Overriding the initial view 


            //Create a WebLayout. By default the version created will be 
            //the latest supported one on the mapguide server we've connected to. For example
            //connecting to MGOS 2.2 will create a version 1.1.0 WebLayout. All the known
            //resource versions have been registered on startup (see Global.asax.cs)
            Version ver = conn.Capabilities.GetMaxSupportedResourceVersion(ResourceTypes.WebLayout.ToString());
            var wl = OSGeo.MapGuide.ObjectModels.ObjectFactory.CreateWebLayout(ver, mdfId);

            //What is IWebLayout2? It is an extension of IWebLayout that supports the ping server property.
            //This is the interface equivalent of WebLayout 1.1.0 schema. Most schema revisions in MapGuide
            //are additive and incremental, and our Object Model interfaces follow the same pattern. All new
            //interfaces inherit from their ancestor interface
            //
            //Anyway, what we want to do is if we created a 1.1.0 WebLayout, is to switch on the ping server
            //property, thus preventing session expiry
            var wl2 = wl as IWebLayout2;
            if (wl2 != null)
                wl2.EnablePingServer = true;

            wl.Title = "Maestro API Web Samples";
            // The initial task pane URL is always assumed to be relative to /mapguide/mapviewer[net|php|java|ajax]
            // Computing an absolute URL here will not work
            wl.TaskPane.InitialTask = "../../Home/TaskPane";

            string resId = "Session:" + conn.SessionID + "//Sheboygan.WebLayout";
            conn.ResourceService.SaveResourceAs(wl, resId);

            // This application is set up to proxy all MG Web Tier requests to the underlying IIS/Apache server, so we
            // can redirect to the AJAX viewer in this origin
            return Redirect("/mapguide/mapviewerajax/?WEBLAYOUT=" + resId + "&SESSION=" + conn.SessionID);
        }
        else
        {
            throw new Exception("Could not find map definition: " + mdfId + ". Please ensure the Sheboygan sample dataset has been loaded");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
