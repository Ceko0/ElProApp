namespace ElProApp.Services.Data
{
    using ElProApp.Data.Models;
    using ElProApp.Data.Repository;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Web.Models.Employee;
    using Interfaces;

    public class EmployeeService(IRepository<Employee, Guid> employeeRepository) : IEmployeeService
    {
        public async Task<bool> AddAsync(EmployeeInputModel model, string userId)
        {
            var existingEmployee = await employeeRepository.FirstOrDefaultAsync(e => e.UserId == userId);
            if (existingEmployee != null)
            {
                throw new InvalidOperationException("Вече имате създаден служител");
            }

            var employee = new Employee
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Wages = model.Wages,
                UserId = userId
            };

            await employeeRepository.AddAsync(employee);
            return true;
        }
    }
}
