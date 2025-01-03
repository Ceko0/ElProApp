namespace ElProApp.Web.Models.Team
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ElProApp.Data.Models;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Building;
    using ElProApp.Web.Models.Employee;
    using ElProApp.Web.Models.JobDone;
    using ElProApp.Web.Models.Job;
    using static ElProApp.Common.EntityValidationConstants.Team;
    using static ElProApp.Common.EntityValidationErrorMessage.Team;
    using static ElProApp.Common.EntityValidationErrorMessage.Master;


    // ViewModel for adding a team
    public class TeamInputModel : IMapTo<Team>
    {
        // Name of the team
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MinLength(NameMinLength, ErrorMessage = ErrorMassageNameMinLength)]
        [MaxLength(NameMaxLength, ErrorMessage = ErrorMassageNameMaxLength)]
        public string Name { get; set; } = null!;

        // Collection of Buildings associated with the team
        public ICollection<BuildingViewModel> BuildingWithTeam { get; set; } = new List<BuildingViewModel>();

        // Collection of JobsList completed by the team
        public ICollection<JobDoneViewModel> JobsDoneByTeam { get; set; } = new List<JobDoneViewModel>();

        // ID of the selected building
        public Guid? SelectedBuildingId { get; set; }

        // List of selected employee IDs for the team
        public List<Guid> SelectedEmployeeIds { get; set; } = new List<Guid>();

        // List of all available employees to display as checkboxes
        public ICollection<EmployeeViewModel> AvailableEmployees { get; set; } = new List<EmployeeViewModel>();

        public ICollection<JobViewModel> AllJobs { get; set; } = new List<JobViewModel>();
    }
}
