namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using ElProApp.Data;
    using ElProApp.Data.Models;
    using ElProApp.Services.Data.Interfaces;

    /// <summary>
    /// Service for performing search operations across various entities in the database.
    /// </summary>
    /// <param name="_data">The database context used for accessing data.</param>
    public class SearchService(ElProAppDbContext _data) : ISearchService
    {
        private readonly ElProAppDbContext data = _data;

        /// <summary>
        /// Performs a search operation based on the provided query and specified entity type.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <param name="searchIn">The specific entity type to search in (e.g., "Employee", "Building"). If null or empty, searches across all entity types.</param>
        /// <returns>
        /// A task representing the asynchronous operation, containing a list of matching results as objects.
        /// </returns>
        public async Task<List<object>> SearchAsync(string query, string searchIn)
        {
            var results = new List<object>();

            if (string.IsNullOrEmpty(searchIn))
            {
                results.AddRange(await SearchEntities<Employee>(query));
                results.AddRange(await SearchEntities<Building>(query));
                results.AddRange(await SearchEntities<JobDone>(query));
                results.AddRange(await SearchEntities<Job>(query));
                results.AddRange(await SearchEntities<Team>(query));
            }
            else
            {
                switch (searchIn)
                {
                    case "Employee":
                        results.AddRange(await SearchEntities<Employee>(query));
                        break;
                    case "Building":
                        results.AddRange(await SearchEntities<Building>(query));
                        break;
                    case "Team":
                        results.AddRange(await SearchEntities<Team>(query));
                        break;
                    case "JobDone":
                        results.AddRange(await SearchEntities<JobDone>(query));
                        break;
                    case "Job":
                        results.AddRange(await SearchEntities<Job>(query));
                        break;
                }
            }
            return results;
        }

        /// <summary>
        /// Searches for entities of a specific type that match the query.
        /// </summary>
        /// <typeparam name="T">The type of entity to search for.</typeparam>
        /// <param name="query">The search query string.</param>
        /// <returns>
        /// A task representing the asynchronous operation, containing a list of matching entities.
        /// </returns>
        private async Task<List<T>> SearchEntities<T>(string query) where T : class
        {
            var entities = await data
                .Set<T>()
                .Where(e => EF.Property<bool>(e, "IsDeleted") == false)
                .ToListAsync();

            var results = entities
                .Where(e => typeof(T)
                .GetProperties()
                    .Any(prop =>
                    {
                        var value = prop.GetValue(e);
                        if (value == null) return false;

                        if (value is string stringValue)
                        {
                            return stringValue.Contains(query, StringComparison.OrdinalIgnoreCase);
                        }
                        if (value is int intValue && int.TryParse(query, out int parsedQuery))
                        {
                            return intValue == parsedQuery;
                        }
                        if (value is decimal decimalValue && decimal.TryParse(query, out decimal parsedDecimal))
                        {
                            return decimalValue == parsedDecimal;
                        }
                        if (value is DateTime dateTimeValue && DateTime.TryParse(query, out DateTime parsedDate))
                        {
                            return dateTimeValue.Date == parsedDate.Date;
                        }

                        return false;
                    }))
                .ToList();

            return results;
        }
    }
}
