using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MvcCoreSample.Models;
using OSGeo.MapGuide.MaestroAPI;
using OSGeo.MapGuide.ObjectModels;
using OSGeo.MapGuide.ObjectModels.WebLayout;
using System;
using System.Diagnostics;

namespace MvcCoreSample.Controllers
{
    public class HomeController : Controller
    {
        readonly AppSettings _options;

        public HomeController(IOptions<AppSettings> opts)
        {
            _options = opts.Value;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Viewer));
        }

        public IActionResult Viewer()
        {
            IServerConnection conn = ConnectionProviderRegistry.CreateConnection(
                "Maestro.Http",
                "Url", _options.MapAgentUrl,
                "Username", "Anonymous",
                "Password", "");

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
                IWebLayout wl = ObjectFactory.CreateWebLayout(ver, mdfId);

                //What is IWebLayout2? It is an extension of IWebLayout that supports the ping server property.
                //This is the interface equivalent of WebLayout 1.1.0 schema. Most schema revisions in MapGuide
                //are additive and incremental, and our Object Model interfaces follow the same pattern. All new
                //interfaces inherit from their ancestor interface
                //
                //Anyway, what we want to do is if we created a 1.1.0 WebLayout, is to switch on the ping server
                //property, thus preventing session expiry
                IWebLayout2 wl2 = wl as IWebLayout2;
                if (wl2 != null)
                    wl2.EnablePingServer = true;

                wl.Title = "Maestro API MVC Core Sample";
                wl.TaskPane.InitialTask = Url.AbsoluteAction(nameof(SampleController.Index), "Sample");

                string resId = $"Session:{conn.SessionID}//Sheboygan.WebLayout";
                conn.ResourceService.SaveResourceAs(wl, resId);

                var model = new SampleViewerModel
                {
                    Title = wl.Title,
                    WebLayout = resId,
                    Session = conn.SessionID
                };
                return View(model);
            }
            else
            {
                throw new Exception("Could not find map definition: " + mdfId + ". Please ensure the Sheboygan sample dataset has been loaded");
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
