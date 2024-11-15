
using AutoMapper;

namespace ElProApp.Web.Models.JobDone
{    
    using Services.Mapping;
    
    using ElProApp.Data.Models;
    using ElProApp.Data.Models.Mappings;

    public class JobDoneViewModel : IMapFrom<JobDone> , IHaveCustomMappings
    {
        public Guid Id { get; set; } 

        public decimal quantity { get; set; }

        public int DaysForJob { get; set; }

        public Guid JobId { get; set; }

        public Job Job { get; set; } = new();

        public virtual ICollection<JobDoneTeamMapping> TeamsDoTheJob { get; set; } = new List<JobDoneTeamMapping>();

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<JobDone, JobDoneViewModel>()
                .ForMember(d => d.TeamsDoTheJob, x => x.MapFrom(s => s.TeamsDoTheJob));
            configuration.CreateMap<JobDone, JobDoneViewModel>()
                .ForMember(d => d.Job, x => x.MapFrom(s => s.Job));
        }
    }
}
