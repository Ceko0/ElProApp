using Microsoft.AspNetCore.Mvc;

namespace ElProApp.Web.Controllers
{
    public class TeamController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
