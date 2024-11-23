namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using ElProApp.Web.Models;

    /// <summary>
    /// Controller responsible for handling the home page and related actions.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">The logger to log messages for this controller.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Displays the home page.
        /// </summary>
        /// <returns>A view displaying the home page.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the privacy page.
        /// </summary>
        /// <returns>A view displaying the privacy policy.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays the error page with error details.
        /// </summary>
        /// <returns>A view displaying the error details.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
