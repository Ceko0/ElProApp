namespace ElProApp.Data.SeedData
{
    using ElProApp.Data.Models.Mappings;
    using Microsoft.AspNetCore.Identity;
    using Newtonsoft.Json;
    using System.IO;
    using System.Linq;

    public class DatabaseSeeder(ElProAppDbContext context, UserManager<IdentityUser> userManager)
    {
        private readonly ElProAppDbContext context = context;
        private readonly UserManager<IdentityUser> userManager = userManager;

        public void SeedDatabase()
        {
            SeedInitialData();
            SeedMappingData();
        }

        private void SeedInitialData()
        {
            var jsonData = File.ReadAllText("D:\\ProgramsScool\\ElProApp\\ElProApp\\ElProApp.Data\\SeedData\\SeedData.json");
            var initialData = JsonConvert.DeserializeObject<InitialData>(jsonData);

            foreach (var team in initialData.Teams)
            {
                if (!context.Teams.Any(x => x.Id == team.Id))
                {
                    context.Teams.Add(team);
                }
            }
            context.SaveChanges();

            foreach (var employee in initialData.Employees)
            {
                var existingUser = userManager.FindByIdAsync(employee.UserId).Result;

                if (existingUser == null)
                {
                    var user = new IdentityUser
                    {
                        Id = employee.UserId,
                        UserName = $"{employee.FirstName}.{employee.LastName}",
                        Email = $"{employee.FirstName}.{employee.LastName}@abv.com",
                        EmailConfirmed = false
                    };

                    var result = userManager.CreateAsync(user, "123456").Result;

                    if (result.Succeeded)
                    {
                        context.Employees.Add(employee);
                    }
                    else
                    {
                        throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    Console.WriteLine($"User with ID {employee.UserId} already exists.");
                }
            }
            context.SaveChanges();

            foreach (var building in initialData.Buildings)
            {
                if (!context.Buildings.Any(x => x.Id == building.Id))
                {
                    context.Buildings.Add(building);
                }
            }
            context.SaveChanges();

            foreach (var job in initialData.Jobs)
            {
                if (!context.Jobs.Any(x => x.Id == job.Id))
                {
                    context.Jobs.Add(job);
                }
            }
            context.SaveChanges();

            foreach (var jobDone in initialData.JobDones)
            {
                if (!context.JobsDone.Any(x => x.Id == jobDone.Id))
                {
                    context.JobsDone.Add(jobDone);
                }
            }
            context.SaveChanges();
        }

        private void SeedMappingData()
        {
            var jsonData = File.ReadAllText("D:\\ProgramsScool\\ElProApp\\ElProApp\\ElProApp.Data\\SeedData\\MappingSeedData.json");
            var mappingData = JsonConvert.DeserializeObject<MappingData>(jsonData);

            foreach (var mapping in mappingData.Mappings)
            {
                // Проверка и добавяне на мапинг на BuildingTeamMapping
                foreach (var buildingId in mapping.BuildingIds)
                {
                    if (!context.BuildingTeamMappings.Any(x => x.BuildingId == buildingId && x.TeamId == mapping.TeamId))
                    {
                        var buildingTeamMapping = new BuildingTeamMapping
                        {
                            BuildingId = buildingId,
                            TeamId = mapping.TeamId
                        };
                        context.BuildingTeamMappings.Add(buildingTeamMapping);
                    }
                }

                // Проверка и добавяне на мапинг на EmployeeTeamMapping
                foreach (var employeeId in mapping.EmployeeIds)
                {
                    if (!context.EmployeeTeamMappings.Any(x => x.EmployeeId == employeeId && x.TeamId == mapping.TeamId))
                    {
                        var employeeTeamMapping = new EmployeeTeamMapping
                        {
                            EmployeeId = employeeId,
                            TeamId = mapping.TeamId
                        };
                        context.EmployeeTeamMappings.Add(employeeTeamMapping);
                    }
                }

                // Проверка и добавяне на мапинг на JobDoneTeamMapping
                foreach (var jobDoneId in mapping.JobDoneIds)
                {
                    if (!context.JobDoneTeamMappings.Any(x => x.JobDoneId == jobDoneId && x.TeamId == mapping.TeamId))
                    {
                        var jobDoneTeamMapping = new JobDoneTeamMapping
                        {
                            JobDoneId = jobDoneId,
                            TeamId = mapping.TeamId
                        };
                        context.JobDoneTeamMappings.Add(jobDoneTeamMapping);
                    }
                }
            }

            context.SaveChanges();
        }
    }
}
