namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using ElProApp.Web.Models;
    using Microsoft.AspNetCore.Http;
    using AspNetCoreGeneratedDocument;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if(!statusCode.HasValue) return View();

            switch (statusCode)
            {
                case 404:
                    return View("_Error404");
                case 500:
                    return View("_Error500");
                case 403:
                    return View("_Error403");
                default:
                    return View("Error");
            }
        }

        
    }
}
