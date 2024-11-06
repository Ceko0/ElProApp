namespace ElProApp.Web.Models.Team
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ElProApp.Data.Models.Mappings;
    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;
    using static Common.EntityValidationConstants.Team;
    using static Common.EntityValidationErrorMessage.Team;
    using static Common.EntityValidationErrorMessage.Master;

    public class TeamEditInputModel : IMapTo<Team>
    {        
        public Guid Id { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MinLength(NameMinLength, ErrorMessage = ErrorMassageNameMinLength)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        public string Name { get; set; } = null!;

        public ICollection<BuildingTeamMapping> BuildingWithTeam { get; set; } = new List<BuildingTeamMapping>();

        public ICollection<JobDoneTeamMapping> JobsDoneByTeam { get; set; } = new List<JobDoneTeamMapping>();

        public ICollection<EmployeeTeamMapping> EmployeesInTeam { get; set; } = new List<EmployeeTeamMapping>();
    }
}
