namespace ElProApp.Web.Models.Job
{
    using ElProApp.Data.Models;
    using ElProApp.Services.Mapping;

    public class JobViewModel : IMapFrom<Job>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }
    }
}
