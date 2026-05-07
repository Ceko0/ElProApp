namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly IEmailSender _emailSender;

            public HomeController(IEmailSender emailSender)
            {
                _emailSender = emailSender;
            }

            public async Task<IActionResult> TestEmail()
            {
                await _emailSender.SendEmailAsync(
                    "falenangel86@gmail.com",
                    "TEST",
                    "<h1>raboti!</h1>");

                return Content("Sent");
            }
        /// <summary>
        /// Displays the home page.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that represents the view for the home page.
        /// </returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays an error page based on the provided HTTP status code.
        /// If no status code is provided, a generic error page is displayed.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for which to display the error page.</param>
        /// <returns>An action result that represents the appropriate error page for the given status code.</returns>
        public IActionResult Error(int? statusCode = null)
        {
            if (!statusCode.HasValue) return View();

            return statusCode switch
            {
                404 => View("Error404"),
                500 => View("Error500"),
                403 => View("Error403"),
                _ => View("Error"),
            };
        }
     
            
        }
    
}
