
namespace ElProApp.Web.Models.JobDone
{    
    using ElProApp.Services.Mapping;
    
    using ElProApp.Data.Models;
    using ElProApp.Data.Models.Mappings;

    public class JobDoneViewModel : IMapFrom<JobDone>
    {
        public Guid Id { get; set; } 

        public decimal quantity { get; set; }

        public int DaysForJob { get; set; }

        public Guid JobId { get; set; }

        public Job Job { get; set; } = new();

        public virtual ICollection<JobDoneTeamMapping> TeamsDoTheJob { get; set; } = new List<JobDoneTeamMapping>();
    }
}
