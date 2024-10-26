using ElProApp.Web.Models.Employee;

namespace ElProApp.Services.Data.Interfaces
{
    public interface IEmployeeService
    {
        public Task<bool> AddAsync(EmployeeInputModel model, string userId);
    }
}
