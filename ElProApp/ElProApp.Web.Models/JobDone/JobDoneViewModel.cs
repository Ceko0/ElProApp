namespace ElProApp.Web.Models.JobDone
{
    using System;
    using System.Collections.Generic;

    using AutoMapper;

    using ElProApp.Data.Models;
    using ElProApp.Services.Mapping;

    /// <summary>
    /// View model for displaying job-done records.
    /// </summary>
    public class JobDoneViewModel : IMapFrom<JobDone>, IHaveCustomMappings
    {
        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets working days count.
        /// </summary>
        public int DaysForJob { get; set; }

        /// <summary>
        /// Gets or sets team identifier.
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// Gets or sets team.
        /// </summary>
        public Team Team { get; set; } = null!;

        /// <summary>
        /// Gets or sets building identifier.
        /// </summary>
        public Guid BuildingId { get; set; }

        /// <summary>
        /// Gets or sets building.
        /// </summary>
        public Building Building { get; set; } = null!;

        /// <summary>
        /// Gets or sets materials with quantities.
        /// </summary>
        public List<MaterialInputPair> Materials { get; set; } = new();

        /// <summary>
        /// Custom mappings.
        /// </summary>
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<JobDone, JobDoneViewModel>()
                .ForMember(d => d.Materials, x => x.Ignore());
        }
    }
}