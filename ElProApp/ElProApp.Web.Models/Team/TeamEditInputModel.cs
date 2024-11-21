namespace ElProApp.Web.Models.Team
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models;
    using static Common.EntityValidationConstants.Team;
    using static Common.EntityValidationErrorMessage.Team;
    using static Common.EntityValidationErrorMessage.Master;
    using ElProApp.Web.Models.Building;
    using ElProApp.Web.Models.Employee;
    using ElProApp.Web.Models.JobDone;

    public class TeamEditInputModel : IMapTo<Team>
    {        
        public Guid Id { get; set; }

        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MinLength(NameMinLength, ErrorMessage = ErrorMassageNameMinLength)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        public string Name { get; set; } = null!;

        public ICollection<BuildingViewModel> BuildingWithTeam { get; set; } = new List<BuildingViewModel>();

        public ICollection<JobDoneViewModel> JobsDoneByTeam { get; set; } = new List<JobDoneViewModel>();

        public ICollection<EmployeeViewModel> EmployeesInTeam { get; set; } = new List<EmployeeViewModel>();

        public List<Guid> BuildingWithTeamIds { get; set; } = new List<Guid>();
        public List<Guid> JobsDoneByTeamIds { get; set; } = new List<Guid>();
        public List<Guid> EmployeesInTeamIds { get; set; } = new List<Guid>();

    }
}
