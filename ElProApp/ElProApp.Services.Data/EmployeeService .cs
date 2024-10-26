namespace ElProApp.Services.Data
{
    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Web.Models.Employee;
    using Interfaces;
    using ElProApp.Services.Mapping;

    public class EmployeeService(IRepository<Employee, Guid> employeeRepository) : IEmployeeService
    {
        public async Task<bool> AddAsync(EmployeeInputModel model, string userId)
        {
            var existingEmployee = await employeeRepository.FirstOrDefaultAsync(e => e.UserId == userId);
            if (existingEmployee != null)
            {
                throw new InvalidOperationException("Вече имате създаден служител");
            }

            var employee = AutoMapperConfig.MapperInstance.Map<Employee>(model);

            employee.UserId = userId;

            await employeeRepository.AddAsync(employee);
            return true;
        }


        public async Task<EmployeeViewModel> GetEmployeeByIdAsync(Guid id)
        {
            var entity = await employeeRepository
                .GetByIdAsync(id);

            EmployeeViewModel model = null;

            if (entity != null) AutoMapperConfig.MapperInstance.Map(entity, model);

            return model;
        }
    }
}
