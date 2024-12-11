namespace ElProApp.Data.SeedData
{
    using ElProApp.Data.Models;
    using Microsoft.AspNetCore.Identity;

    public class InitialData
    {
        public List<Team> Teams { get; set; } = [];
        public List<Employee> Employees { get; set; } = [];
        public List<Building> Buildings { get; set; } = [];
        public List<Job> Jobs { get; set; } = [];
        public List<JobDone> JobDones { get; set; } = [];
        public List<IdentityUser> IdentityUsers { get; set; } = [];

    }
}
