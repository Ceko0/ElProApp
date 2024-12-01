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
            SeedUsers();
            SeedInitialData();
            SeedMappingData();
        }

        private void SeedUsers()
        {
            var jsonData = File.ReadAllText(@"..\ElProApp.Data\SeedData\SeedData.json");
            var initialData = JsonConvert.DeserializeObject<InitialData>(jsonData);

            foreach (var user in initialData.IdentityUsers)
            {
                var existingUser = userManager.FindByIdAsync(user.Id).Result;

                if (existingUser == null)
                {
                    var newUser = new IdentityUser
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        EmailConfirmed = false,
                        PhoneNumber = user.PhoneNumber 
                    };

                    var result = userManager.CreateAsync(newUser, "123456").Result;

                    if (result.Succeeded)
                    {
                        Console.WriteLine($"User with ID {user.Id} created successfully.");
                        context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine($"Failed to create user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    Console.WriteLine($"User with ID {user.Id} already exists.");
                }
            }
        }

        private void SeedInitialData()
        {
            var jsonData = File.ReadAllText(@"..\ElProApp.Data\SeedData\SeedData.json");
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
                if (!context.Employees.Any(x => x.Id == employee.Id))
                {
                    context.Employees.Add(employee);
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
            var jsonData = File.ReadAllText($@"..\ElProApp.Data\SeedData\MappingSeedData.json");
            var mappingData = JsonConvert.DeserializeObject<MappingData>(jsonData);

            foreach (var mapping in mappingData.Mappings)
            {
                
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
