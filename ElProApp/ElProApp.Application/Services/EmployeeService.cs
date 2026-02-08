namespace ElProApp.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Application.Services.Interfaces;
    using ElProApp.Web.Models.Employee;

    /// <summary>
    /// Provides application-level operations for managing employees.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee, Guid> employeeRepository;
        private readonly IEmployeeTeamMappingService employeeTeamMappingService;
        private readonly IHelpMethodsService helpMethodsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeService"/> class.
        /// </summary>
        public EmployeeService(
            IRepository<Employee, Guid> employeeRepository,
            IEmployeeTeamMappingService employeeTeamMappingService,
            IHelpMethodsService helpMethodsService)
        {
            this.employeeRepository = employeeRepository;
            this.employeeTeamMappingService = employeeTeamMappingService;
            this.helpMethodsService = helpMethodsService;
        }

        /// <summary>
        /// Creates a new employee for the current user.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the model is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing.</exception>
        /// <exception cref="InvalidOperationException">Thrown when an employee already exists.</exception>
        public async Task<string> AddAsync(EmployeeInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (string.IsNullOrWhiteSpace(model.Name) ||
                string.IsNullOrWhiteSpace(model.LastName))
            {
                throw new ArgumentException(
                    "Employee first name and last name must be provided.");
            }

            string? userId = helpMethodsService.GetUserId();

            bool exists = await employeeRepository
                .GetAllAttached()
                .AnyAsync(e => e.UserId == userId && !e.IsDeleted);

            if (exists)
                throw new InvalidOperationException(
                    "Employee already exists for this user.");

            var employee = AutoMapperConfig.MapperInstance.Map<Employee>(model);
            employee.UserId = userId;

            await employeeRepository.AddAsync(employee);
            await employeeRepository.SaveAsync();

            return employee.Id.ToString();
        }

        /// <summary>
        /// Retrieves an employee by identifier.
        /// </summary>
        public async Task<EmployeeViewModel?> GetByIdAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var entity = await employeeRepository.GetByIdAsync(validId)
                ?? throw new InvalidOperationException(
                    "Employee not found.");

            if (entity.IsDeleted)
                throw new InvalidOperationException(
                    "Employee is deleted.");

            entity.User = await helpMethodsService.GetUserAsync(entity.UserId);

            var model =
                AutoMapperConfig.MapperInstance.Map<EmployeeViewModel>(entity);

            model.TeamsEmployeeBelongsTo =
                employeeTeamMappingService.GetAllByEmployeeId(id).ToList();

            return model;
        }

        /// <summary>
        /// Retrieves an employee for edit by identifier.
        /// </summary>
        public async Task<EmployeeEditInputModel> EditByIdAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);

            var entity = await employeeRepository.GetByIdAsync(validId)
                ?? throw new InvalidOperationException(
                    "Employee not found.");

            if (entity.IsDeleted)
                throw new InvalidOperationException(
                    "Employee is deleted.");

            IdentityUser user = !string.IsNullOrWhiteSpace(entity.UserId)
                ? await helpMethodsService.GetUserAsync(entity.UserId)
                    ?? throw new InvalidOperationException("User not found.")
                : await helpMethodsService.GetCurrentUserAsync();

            entity.User = user;

            return AutoMapperConfig.MapperInstance
                .Map<EmployeeEditInputModel>(entity);
        }

        /// <summary>
        /// Updates employee data.
        /// </summary>
        public async Task<bool> EditByModelAsync(EmployeeEditInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var entity = await employeeRepository.GetByIdAsync(model.Id)
                ?? throw new InvalidOperationException(
                    "Employee not found.");

            if (entity.IsDeleted)
                throw new InvalidOperationException(
                    "Employee is deleted.");

            AutoMapperConfig.MapperInstance.Map(model, entity);
            await employeeRepository.SaveAsync();

            return true;
        }

        /// <summary>
        /// Retrieves all active employees.
        /// </summary>
        public async Task<ICollection<EmployeeViewModel>> GetAllAsync()
        {
            var employees = await employeeRepository
                .GetAllAttached()
                .Where(x => !x.IsDeleted)
                .To<EmployeeViewModel>()
                .ToListAsync();

            foreach (var employee in employees)
            {
                employee.TeamsEmployeeBelongsTo =
                    employeeTeamMappingService
                        .GetAllByEmployeeId(employee.Id.ToString());
            }

            return employees;
        }

        /// <summary>
        /// Returns all attached, non-deleted employees.
        /// </summary>
        public IQueryable<Employee> GetAllAttached()
            => employeeRepository
                .GetAllAttached()
                .Include(e => e.User)
                .Where(e => !e.IsDeleted);

        /// <summary>
        /// Soft deletes an employee.
        /// </summary>
        public async Task<bool> SoftDeleteAsync(string id)
        {
            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);
            return await employeeRepository.SoftDeleteAsync(validId);
        }

        /// <summary>
        /// Retrieves an employee by user identifier.
        /// </summary>
        public Employee GetByUserId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(
                    "Invalid user ID.", nameof(id));

            return employeeRepository
                .GetAllAttached()
                .FirstOrDefault(x =>
                    x.UserId == id && !x.IsDeleted)
                ?? throw new InvalidOperationException(
                    "Employee not found for the specified user.");
        }

        /// <summary>
        /// Creates an administrative employee.
        /// </summary>
        public async Task<string> AddAdminEmployeeAsync(
            string firstName,
            string lastName,
            string identityUserId)
        {
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException(
                    "First name and last name are required.");
            }

            var employee = new Employee
            {
                Name = firstName,
                LastName = lastName,
                Wages = 10,
                UserId = identityUserId
            };

            await employeeRepository.AddAsync(employee);
            await employeeRepository.SaveAsync();

            return employee.Id.ToString();
        }

        /// <summary>
        /// Persists pending employee changes.
        /// </summary>
        public async Task<bool> SaveChangesAsync()
            => await employeeRepository.SaveAsync();
    }
}
