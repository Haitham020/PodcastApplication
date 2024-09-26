using Microsoft.AspNetCore.Mvc;

namespace PodcastApplication.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class ControlPanelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
