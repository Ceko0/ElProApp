namespace ElProApp.Services.Data
{
    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Web.Models.Employee;
    using Interfaces;
    using ElProApp.Services.Mapping;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;

    public class EmployeeService(IRepository<Employee, Guid> employeeRepository,UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor) : IEmployeeService
    {
        private readonly IRepository<Employee, Guid> employeeRepository = employeeRepository;
        private readonly UserManager<IdentityUser> userManaget = userManager;
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;


        /// <summary>
        /// Adds a new employee.
        /// </summary>
        /// <param name="model">The employee input model.</param>
        /// <returns>ID of the newly created employee.</returns>
        public async Task<string> AddAsync(EmployeeInputModel model)
        {
            string? userId = GetUserId();

            var existingEmployee = await employeeRepository.FirstOrDefaultAsync(e => e.UserId == userId);
            if (existingEmployee != null) throw new InvalidOperationException("Вече имате създаден служител");

            var employee = AutoMapperConfig.MapperInstance.Map<Employee>(model);

            employee.UserId = userId;

            await employeeRepository.AddAsync(employee);
            return employee.Id.ToString();
        }

        /// <summary>
        /// Retrieves employee information by ID.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>The employee view model.</returns>
        public async Task<EmployeeViewModel?> GetByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);

            var entity = await employeeRepository.GetByIdAsync(validId);
            return entity != null ? AutoMapperConfig.MapperInstance.Map<EmployeeViewModel>(entity) : throw new ArgumentException("mising entity.");
        }

        /// <summary>
        /// Retrieves the edit model for an employee.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>The edit model for the employee.</returns>
        public async Task<EmployeeEditInputModel> EditByModelAsync(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId)) throw new ArgumentException("Invalid ID format.");
            
            var entity = await employeeRepository.GetByIdAsync(validId);

            if ( entity.IsDeleted) throw new InvalidOperationException("User is deleted.");

            IdentityUser user;
           
            if (!string.IsNullOrEmpty(entity.UserId))
            {                
                user = await userManager.FindByIdAsync(entity.UserId)
                       ?? throw new InvalidOperationException("User not found.");
            }
            else user = await GetCurrentUserAsync();

            entity.User = user;

            return AutoMapperConfig.MapperInstance.Map<Employee, EmployeeEditInputModel>(entity);

            //entity == null 
            //    ? AutoMapperConfig.MapperInstance.Map<EmployeeEditInputModel>(entity)
            //    : throw new ArgumentException("mising entity.");

        }

        /// <summary>
        /// Edits the information of an employee.
        /// </summary>
        /// <param name="model">The model with new employee data.</param>
        /// <returns>True if the edit was successful.</returns>
        public async Task<bool> EditByModelAsync(EmployeeEditInputModel model)
        {
            try
            {
                var entity = await employeeRepository.GetByIdAsync(model.Id) ?? throw new ArgumentException("Entity not found.");
                AutoMapperConfig.MapperInstance.Map(model, entity);

                await employeeRepository.SaveAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        /// <summary>
        /// Retrieves all employees.
        /// </summary>
        /// <returns>A list of all employees.</returns>
        public async Task<IEnumerable<EmployeeAllViewModel>> GetAllAsync()
        {
            var model = await employeeRepository.GetAllAttached().Where(x => !x.IsDeleted).To<EmployeeAllViewModel>().ToArrayAsync();

            return model;
        }

        /// <summary>
        /// Soft deletes an employee by ID.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>True if the deletion was successful.</returns>
        public async Task<bool> SoftDeleteAsync(string id)
        {
            try
            {  
                Guid validId = ConvertAndTestIdToGuid(id);
                bool isDeleted = await employeeRepository.SoftDeleteAsync(validId);

                return isDeleted;
            }
            catch (Exception)
            {
                return false;
            }             
        }

        /// <summary>
        /// Retrieves the ID of the current user.
        /// </summary>
        /// <returns>The ID of the user.</returns>
        private string GetUserId()
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("Неуспешно извличане на UserId. Опитай отново.");
            return userId;
        }

        /// <summary>
        /// Converts the ID to a Guid and checks its validity.
        /// </summary>
        /// <param name="id">The employee's ID.</param>
        /// <returns>A valid Guid.</returns>
        private static Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId)) throw new ArgumentException("Invalid ID format.");
            return validId;
        }

        private async Task<IdentityUser> GetCurrentUserAsync()
        {
            var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) throw new InvalidOperationException("User not found.");

            return await userManager.FindByIdAsync(userId)
                   ?? throw new InvalidOperationException("Invalid user.");
        }
    }
}
