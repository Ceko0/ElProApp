using ElProApp.Web.Models.Employee;

namespace ElProApp.Services.Data.Interfaces
{
    public interface IEmployeeService
    {
        public Task<string> AddAsync(EmployeeInputModel model);

        public Task<EmployeeViewModel?> GetEmployeeByIdAsync(string id);
    }
}
