namespace ElProApp.Services.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Http;
    using ElProApp.Data.Repository.Interfaces;
    using System.Security.Claims; 
    using Microsoft.EntityFrameworkCore;

    using Web.Models.Employee;
    using Services.Data.Interfaces;
    using Services.Mapping;
    using ElProApp.Data.Models;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Service class for managing employee-related operations, including adding, editing, deleting, and retrieving employee information.
    /// </summary>
    public class EmployeeService(IRepository<Employee, Guid> _employeeRepository
            , UserManager<IdentityUser> _userManager
            , IHttpContextAccessor _httpContextAccessor
            , IServiceProvider _serviceProvider)
            : IEmployeeService
    {
        private readonly IRepository<Employee, Guid> employeeRepository = _employeeRepository;
        private readonly UserManager<IdentityUser> userManager = _userManager;
        private readonly IHttpContextAccessor httpContextAccessor = _httpContextAccessor;
        private readonly IServiceProvider serviceProvider = _serviceProvider;

        /// <summary>
        /// Adds a new employee based on the provided model.
        /// </summary>
        /// <param name="model">The employee input model containing required data.</param>
        /// <returns>The ID of the newly created employee.</returns>
        public async Task<string> AddAsync(EmployeeInputModel model)
        {
            string? userId = GetUserId();

            var existingEmployee = await employeeRepository.FirstOrDefaultAsync(e => e.UserId == userId);
            if (existingEmployee != null) throw new InvalidOperationException("Employee already exists for this user.");

            var employee = AutoMapperConfig.MapperInstance.Map<Employee>(model);
            employee.UserId = userId;

            await employeeRepository.AddAsync(employee);
            return employee.Id.ToString();
        }

        /// <summary>
        /// Retrieves employee information by their ID.
        /// </summary>
        /// <param name="id">The ID of the employee to retrieve.</param>
        /// <returns>Returns EmployeeViewModel or throws an exception if the employee is not found.</returns>
        public async Task<EmployeeViewModel?> GetByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var entity = await employeeRepository.GetByIdAsync(validId);            
            entity.User = await GetUserAsync(entity.UserId);

            if (entity == null) throw new ArgumentException("Missing entity.");

            var model = AutoMapperConfig.MapperInstance.Map<EmployeeViewModel>(entity);

            var employeeTeamMappingService = serviceProvider.GetRequiredService<EmployeeTeamMappingService>();
            var teams = employeeTeamMappingService.GetAllByEmployeeId(id).ToList();
            model.TeamsEmployeeBelongsTo = teams;
            
            return model;
        }

        /// <summary>
        /// Retrieves the edit model for an employee with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the employee.</param>
        /// <returns>The input model for editing the employee or an exception if the record is missing.</returns>
        public async Task<EmployeeEditInputModel> EditByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var entity = await employeeRepository.GetByIdAsync(validId);
            if (entity.IsDeleted) throw new InvalidOperationException("User is deleted.");

            IdentityUser user = !string.IsNullOrEmpty(entity.UserId)
                ? await userManager.FindByIdAsync(entity.UserId) ?? throw new InvalidOperationException("User not found.")
                : await GetCurrentUserAsync();

            entity.User = user;
            return AutoMapperConfig.MapperInstance.Map<EmployeeEditInputModel>(entity);
        }

        /// <summary>
        /// Edits employee information based on the provided input model.
        /// </summary>
        /// <param name="model">The model containing updated employee data.</param>
        /// <returns>True if the edit was successful, otherwise False.</returns>
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
        /// Retrieves a list of all employees who are not marked as deleted.
        /// </summary>
        /// <returns>A list of view models representing all active employees.</returns>
        public async Task<ICollection<EmployeeViewModel>> GetAllAsync() 
            => await employeeRepository
                            .GetAllAttached()                            
                            .Where(x => !x.IsDeleted)
                            .To<EmployeeViewModel>()
                            .ToListAsync();

        public IQueryable<Employee> GetAllAttached() 
            => employeeRepository
            .GetAllAttached();

        /// <summary>
        /// Soft deletes an employee by marking them as deleted, based on their ID.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        /// <returns>True if the operation was successful, otherwise False.</returns>
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
        /// Retrieves the ID of the currently logged-in user from their claims.
        /// </summary>
        /// <returns>The user ID.</returns>
        private string GetUserId()
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("Failed to retrieve UserId. Please try again.");
            return userId;
        }

        /// <summary>
        /// Converts a string ID to a Guid and validates its format.
        /// </summary>
        /// <param name="id">The string format ID of the employee.</param>
        /// <returns>A valid Guid.</returns>
        private static Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId)) throw new ArgumentException("Invalid ID format.");
            return validId;
        }

        /// <summary>
        /// Retrieves the currently logged-in user through UserManager if no employee-specific UserId is set.
        /// </summary>
        /// <returns>The IdentityUser representing the current user.</returns>
        private async Task<IdentityUser> GetCurrentUserAsync()
        {
            var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) throw new InvalidOperationException("User not found.");

            return await userManager.FindByIdAsync(userId) ?? throw new InvalidOperationException("Invalid user.");
        }
        private async Task<IdentityUser> GetUserAsync(string id) 
            => await userManager.FindByIdAsync(id) 
            ?? throw new InvalidOperationException("Invalid user.");

        public Employee GetByUserId(string id)
        {
            var entity = employeeRepository.GetAllAttached().Where(x => x.UserId == id).ToArray();

            return entity[0];
        }
    }
}