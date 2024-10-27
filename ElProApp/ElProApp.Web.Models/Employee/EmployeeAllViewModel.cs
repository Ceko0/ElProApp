namespace ElProApp.Web.Models.Employee
{
    using Services.Mapping;

    using Data.Models.Mappings;    

    public class EmployeeAllViewModel : IMapFrom<Data.Models.Employee>
    {
        public string Id { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public decimal Wages { get; set; }
        public decimal MoneyToTake { get; set; }
        public string UserName { get; set; } = null!;
        public ICollection<EmployeeTeamMapping> TeamsEmployeeBelongsTo { get; set; } = [];
    }
}
