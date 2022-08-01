using Microsoft.AspNetCore.Mvc;
using MvcCoreSample.Models;

namespace MvcCoreSample.Controllers
{
    public class SampleController : Controller
    {
        public IActionResult Index(MapGuideViewerModel model) => View(model);
    }
}
