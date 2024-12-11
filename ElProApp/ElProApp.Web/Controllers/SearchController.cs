namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using ElProApp.Services.Data.Interfaces;

    /// <summary>
    /// Controller responsible for handling search operations.
    /// </summary>
    /// <param name="_SearchService">The search service used for performing search operations.</param>
    [Authorize]
    public class SearchController(ISearchService _SearchService) : Controller
    {
        private readonly ISearchService SearchService = _SearchService;

        /// <summary>
        /// Handles search requests and returns the search results.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <param name="searchIn">The specific entity type to search in (e.g., "Employee", "Building" , "JobDone" ,"Job" , "Team").</param>
        /// <returns>A view displaying the search results.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
        public async Task<IActionResult> Search(string query, string searchIn, int pageNumber = 1, int pageSize = 10)
        {
            var (results, totalResults) = await SearchService.SearchAsync(query, searchIn, pageNumber, pageSize);

            int totalPages = (int)Math.Ceiling((double)totalResults / pageSize);

            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchQuery"] = query;
            ViewData["SearchIn"] = searchIn;

            return View(results);
        }

    }
}
