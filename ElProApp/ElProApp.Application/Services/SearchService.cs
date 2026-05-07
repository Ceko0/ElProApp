namespace ElProApp.Application.Services
{
    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data;
    using ElProApp.Application.Services.Interfaces;
    using ElProApp.Web.Models.Search;

    public class SearchService : ISearchService
    {
        private readonly ElProAppDbContext data;

        public SearchService(ElProAppDbContext data)
        {
            this.data = data;
        }

        public async Task<(List<SearchResultViewModel> Results, int TotalResults)>
            SearchAsync(string query, string searchIn, int pageNumber, int pageSize)
        {
            query = query ?? "";

            var results = new List<SearchResultViewModel>();

            if (string.IsNullOrEmpty(searchIn) || searchIn == "All")
            {
                results.AddRange(await SearchEmployees(query));
                results.AddRange(await SearchBuildings(query));
                results.AddRange(await SearchTeams(query));
                results.AddRange(await SearchJobDones(query));
                results.AddRange(await SearchMaterials(query));
            }
            else
            {
                switch (searchIn)
                {
                    case "Employee":
                        results.AddRange(await SearchEmployees(query));
                        break;
                    case "Building":
                        results.AddRange(await SearchBuildings(query));
                        break;
                    case "Team":
                        results.AddRange(await SearchTeams(query));
                        break;                   
                    case "JobDone":
                        results.AddRange(await SearchJobDones(query));
                        break;
                    case "Material":
                        results.AddRange(await SearchMaterials(query));
                        break;
                }
            }

            results = results
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Name)
                .ToList();

            var totalResults = results.Count;

            var pagedResults = results
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (pagedResults, totalResults);
        }

        private async Task<List<SearchResultViewModel>> SearchEmployees(string query)
        {
            return await data.Employees
                .Where(x => !x.IsDeleted &&
                    (EF.Functions.Like(x.Name, $"%{query}%") ||
                     EF.Functions.Like(x.LastName, $"%{query}%")))
                .Select(x => new SearchResultViewModel
                {
                    Id = x.Id.ToString(),
                    Name = x.Name + " " + x.LastName,
                    Type = "Employee",
                    AdditionalInfo = x.LastName
                })
                .ToListAsync();
        }

        private async Task<List<SearchResultViewModel>> SearchBuildings(string query)
        {
            return await data.Buildings
                .Where(x => !x.IsDeleted &&
                    (EF.Functions.Like(x.Name, $"%{query}%") ||
                     EF.Functions.Like(x.Location, $"%{query}%")))
                .Select(x => new SearchResultViewModel
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Type = "Building",
                    AdditionalInfo = x.Location
                })
                .ToListAsync();
        }

        private async Task<List<SearchResultViewModel>> SearchTeams(string query)
        {
            return await data.Teams
                .Where(x => !x.IsDeleted &&
                    EF.Functions.Like(x.Name, $"%{query}%"))
                .Select(x => new SearchResultViewModel
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Type = "Team",
                    AdditionalInfo = x.CreatedDate.ToString("dd/MM/yyyy")
                })
                .ToListAsync();
        }
        
        private async Task<List<SearchResultViewModel>> SearchJobDones(string query)
        {
            return await data.JobsDone
                .Where(x => !x.IsDeleted &&
                    EF.Functions.Like(x.Name, $"%{query}%"))
                .Select(x => new SearchResultViewModel
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Type = "JobDone",
                    AdditionalInfo = x.CreatedDate.ToString("dd/MM/yyyy")
                })
                .ToListAsync();
        }

        private async Task<List<SearchResultViewModel>> SearchMaterials(string query)
        {
            return await data.Materials
                .Include(x => x.Buildings)
                .Where(x => !x.IsDeleted &&
                    (EF.Functions.Like(x.Name, $"%{query}%") ))
                .Select(x => new SearchResultViewModel
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Type = "Material"
                })
                .ToListAsync();
        }
    }
}