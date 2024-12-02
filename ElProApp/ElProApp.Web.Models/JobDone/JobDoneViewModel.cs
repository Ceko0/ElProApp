
using AutoMapper;

namespace ElProApp.Web.Models.JobDone
{    
    using Services.Mapping;
    
    using ElProApp.Data.Models;
    
    public class JobDoneViewModel : IMapFrom<JobDone>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal quantity { get; set; }

        public int DaysForJob { get; set; }

        public Guid JobId { get; set; }

        public Job Job { get; set; } = new();

        public Guid TeamId { get; set; }

        public Team Team { get; set; } = null!;

        public Guid BuildingId { get; set; }

        public Building Building { get; set; } = null!;
       
    }
}
