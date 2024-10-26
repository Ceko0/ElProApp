namespace ElProApp.Services.Data
{
    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Web.Models.Employee;
    using Interfaces;
    using ElProApp.Services.Mapping;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;

    public class EmployeeService(IRepository<Employee, Guid> employeeRepository, IHttpContextAccessor httpContextAccessor) : IEmployeeService
    {
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
        public async Task<bool> AddAsync(EmployeeInputModel model)
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("Неуспешно извличане на UserId. Опитай отново.");
            
            var existingEmployee = await employeeRepository.FirstOrDefaultAsync(e => e.UserId == userId);
            if (existingEmployee != null)  throw new InvalidOperationException("Вече имате създаден служител");
            
            var employee = AutoMapperConfig.MapperInstance.Map<Employee>(model);

            employee.UserId = userId;

            await employeeRepository.AddAsync(employee);
            return true;
        }

        public async Task<EmployeeViewModel?> GetEmployeeByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId))
            {
                throw new ArgumentException("Invalid ID format.");
            }

            var entity = await employeeRepository.GetByIdAsync(validId);
            return entity != null ? AutoMapperConfig.MapperInstance.Map<EmployeeViewModel>(entity) : null;
        }
    }
}
