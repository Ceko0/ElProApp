using Microsoft.AspNetCore.Mvc;

namespace ElProApp.Web.Controllers
{
    public class TeamController : Controller
    {
        public IActionResult Add()
        {
            return View();
        }
    }
}
