namespace ElProApp.Services.Data.Interfaces
{
    /// <summary>
    /// Provides functionality for performing search operations.
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Asynchronously performs a search operation based on the provided query and target.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <param name="searchIn">The target or context to search within.</param>
        /// <returns>A task that represents the asynchronous operation, containing a list of search results.</returns>
        public Task<(List<object> Results, int TotalResults)> SearchAsync(string query, string searchIn, int pageNumber, int pageSize);
    }
}
