namespace ElProApp.Data.SeedData
{
    using Data.Models;
    public class InitialData
    {
        public List<Team> Teams { get; set; } = [];
        public List<Employee> Employees { get; set; } = [];
        public List<Building> Buildings { get; set; } = [];
        public List<Job> Jobs { get; set; } = [];
        public List<JobDone> JobDones { get; set; } = [];

    }
}
