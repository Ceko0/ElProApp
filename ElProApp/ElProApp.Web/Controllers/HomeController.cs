namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using ElProApp.Web.Models;
    using Microsoft.AspNetCore.Http;

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
            statusCode ??= HttpContext.Response.StatusCode;

            switch (statusCode)
            {
                case 404:
                    return Handle404();
                case 500:
                    return Handle500();
                case 403:
                    return Handle403();
                default:
                    return HandleGenericError();
            }
        }

        private IActionResult Handle404()
        {
            var errorModel = new ErrorViewModel
            {
                ErrorMessage = "Sorry, the page you are looking for could not be found.",
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View("_Error404", errorModel); 
        }

        private IActionResult Handle500()
        {
            var errorModel = new ErrorViewModel
            {
                ErrorMessage = "An internal server error occurred.",
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View("_Error500", errorModel);
        }

        private IActionResult Handle403()
        {
            var errorModel = new ErrorViewModel
            {
                ErrorMessage = "You do not have permission to view this page.",
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View("_Error403", errorModel);
        }

        private IActionResult HandleGenericError()
        {
            var errorModel = new ErrorViewModel
            {
                ErrorMessage = "An unexpected error occurred.",
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View("Error", errorModel);
        }
    }
}
