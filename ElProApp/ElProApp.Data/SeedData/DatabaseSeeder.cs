namespace ElProApp.Data.SeedData
{
    using Microsoft.AspNetCore.Identity;
    using Newtonsoft.Json;
    using System.IO;
    using System.Linq; // Не забравяйте да добавите this за LINQ операции

    public class DatabaseSeeder
    {
        private readonly ElProAppDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public DatabaseSeeder(ElProAppDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public void SeedDatabase()
        {
            var jsonData = File.ReadAllText("D:\\ProgramsScool\\ElProApp\\ElProApp\\ElProApp.Data\\SeedData\\SeedData.json");
            var initialData = JsonConvert.DeserializeObject<InitialData>(jsonData);

            foreach (var team in initialData.Teams)
            {
                if (!context.Teams.Any(x => x.Id == team.Id)) context.Teams.Add(team);                
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

                    if (result.Succeeded) context.Employees.Add(employee);                    
                    else throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    
                }
                else Console.WriteLine($"User with ID {employee.UserId} already exists.");                
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
                if (!context.Jobs.Any(x => x.Id == job.Id)) context.Jobs.Add(job);                
            }
            context.SaveChanges();


            foreach (var jobDone in initialData.JobDones)
            {
                if (!context.JobsDone.Any(x => x.Id == jobDone.Id)) context.JobsDone.Add(jobDone);                
            }
            context.SaveChanges();
        }
    }
}
