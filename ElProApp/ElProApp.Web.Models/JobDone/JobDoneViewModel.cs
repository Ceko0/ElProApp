namespace ElProApp.Web.Models.JobDone
{    
    using Services.Mapping;
    using AutoMapper;
    
    using ElProApp.Data.Models;
    
    public class JobDoneViewModel : IMapFrom<JobDone> , IHaveCustomMappings
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public int DaysForJob { get; set; }

        public Dictionary<Guid, decimal> Jobs { get; set; } = new();

        public Guid TeamId { get; set; }

        public Team Team { get; set; } = null!;

        public Guid BuildingId { get; set; }

        public Building Building { get; set; } = null!;

        public virtual ICollection<Job> JobsList { get; set; } = new List<Job>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<JobDone, JobDoneViewModel>()
                .ForMember(d => d.Jobs, x => x.Ignore());
        }
    }
}
