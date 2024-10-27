namespace ElProApp.Web.Models.Team
{
    using System.ComponentModel.DataAnnotations;
 
    using Data.Models.Mappings;
    using static Common.EntityValidationConstants.Team;
    using static Common.EntityValidationErrorMessage.Team;
    using static Common.EntityValidationErrorMessage.Master;

    // ViewModel for adding a team
    public class TeamAddInputModel
    {
        // Unique identifier for the team
        [Required(ErrorMessage = ErrorMassageFieldIsRequired)]        
        public string Id { get; set; }

        // Name of the team
        [Required(ErrorMessage = ErrorMassageFieldForNameIsRequired)]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        // Collection of buildings associated with the team
        public IEnumerable<BuildingTeamMapping> BuildingWithTeam { get; set; } = [];

        // Collection of jobs completed by the team
        public IEnumerable<JobDoneTeamMapping> JobsDoneByTeam { get; set; } = [];

        // Collection of employees who are part of the team
        public IEnumerable<EmployeeTeamMapping> EmployeesInTeams { get; set; } = [];
    }
}
