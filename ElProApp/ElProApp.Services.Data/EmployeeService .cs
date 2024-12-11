using Microsoft.Extensions.DependencyInjection;

namespace ElProApp.Services.Data
{
    using System;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Repository.Interfaces;
    using Web.Models.Employee;
    using Interfaces;
    using Mapping;
    using ElProApp.Data.Models;


    /// <summary>
    /// Service class for managing employee-related operations, including adding, editing, deleting, and retrieving employee information.
    /// </summary>
    public class EmployeeService(IRepository<Employee, Guid> employeeRepository,
                               IEmployeeTeamMappingService employeeTeamMappingService,
                               IHelpMethodsService helpMethodsService)
                               : IEmployeeService 
    {
        
        /// <summary>
        /// Adds a new employee based on the provided model.
        /// </summary>
        /// <param name="model">The employee input model containing required data.</param>
        /// <returns>The ID of the newly created employee.</returns>
        public async Task<string> AddAsync(EmployeeInputModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.LastName))
                throw new ArgumentException("Employee first name must be provided.");

            string? userId = helpMethodsService.GetUserId();

            var existingEmployee = await employeeRepository.FirstOrDefaultAsync(e => e.UserId == userId);
            if (existingEmployee != null) throw new InvalidOperationException("Employee already exists for this user.");

            var employee = AutoMapperConfig.MapperInstance.Map<Employee>(model);
            employee.UserId = userId;

            await employeeRepository.AddAsync(employee);
            await employeeRepository.SaveAsync();

            return employee.Id.ToString();
        }

        /// <summary>
        /// Retrieves employee information by their ID.
        /// </summary>
        /// <param name="id">The ID of the employee to retrieve.</param>
        /// <returns>Returns EmployeeViewModel or throws an exception if the employee is not found.</returns>
        public async Task<EmployeeViewModel?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new InvalidOperationException("Invalid employee ID.");

            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);
            var entity = await employeeRepository.GetByIdAsync(validId);
            if (entity == null || entity.IsDeleted) throw new InvalidOperationException("Employee is deleted or not found.");

            entity.User = await helpMethodsService.GetUserAsync(entity.UserId);

            var model = AutoMapperConfig.MapperInstance.Map<EmployeeViewModel>(entity);

            
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
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Invalid employee ID.");

            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);
            var entity = await employeeRepository.GetByIdAsync(validId);
            if (entity == null || entity.IsDeleted) throw new InvalidOperationException("Employee is deleted or not found.");

            IdentityUser user = !string.IsNullOrEmpty(entity.UserId)
                ? await helpMethodsService.GetUserAsync(entity.UserId) ?? throw new InvalidOperationException("User not found.")
                : await helpMethodsService.GetCurrentUserAsync();

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
            if (model == null) throw new ArgumentNullException(nameof(model));

            var entity = await employeeRepository.GetByIdAsync(model.Id);
            if (entity == null || entity.IsDeleted) throw new InvalidOperationException("Employee is deleted or not found.");

            AutoMapperConfig.MapperInstance.Map(model, entity);

            await employeeRepository.SaveAsync();
            return true;
        }

        /// <summary>
        /// Retrieves a list of all employees who are not marked as deleted.
        /// </summary>
        /// <returns>A list of view models representing all active employees.</returns>
        public async Task<ICollection<EmployeeViewModel>> GetAllAsync()
        {
            var model = await employeeRepository
                            .GetAllAttached()
                            .Where(x => !x.IsDeleted)
                            .To<EmployeeViewModel>()
                            .ToListAsync();

            foreach (var employee in model)
            {
                employee.TeamsEmployeeBelongsTo = employeeTeamMappingService.GetAllByEmployeeId(employee.Id.ToString());
            }

            return model;
        }

        /// <summary>
        /// Retrieves all employees including attached entities.
        /// </summary>
        /// <returns>An IQueryable collection of employees.</returns>
        public IQueryable<Employee> GetAllAttached()
        {
            return employeeRepository
                   .GetAllAttached()
                   .Include(e => e.User)
                   .Where(e => !e.IsDeleted);
        }

        /// <summary>
        /// Soft deletes an employee by marking them as deleted, based on their ID.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        /// <returns>True if the operation was successful, otherwise False.</returns>
        public async Task<bool> SoftDeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Invalid employee ID.");

            Guid validId = helpMethodsService.ConvertAndTestIdToGuid(id);
            bool isDeleted = await employeeRepository.SoftDeleteAsync(validId);

            var currentUser = await helpMethodsService.GetCurrentUserAsync();

            if (currentUser == null)
                throw new InvalidOperationException("Unable to retrieve user context."); 

            return isDeleted;
        }
        
        /// <summary>
        /// Retrieves the employee associated with a user ID.
        /// </summary>
        /// <param name="id">The user ID to search for.</param>
        /// <returns>The associated <see cref="Employee"/> object.</returns>
        public Employee GetByUserId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Invalid user ID.");

            var entity = employeeRepository.GetAllAttached().Where(x => x.UserId == id && !x.IsDeleted).FirstOrDefault();
            if (entity == null) throw new InvalidOperationException("Employee not found for the specified user.");

            return entity;
        }

        /// <summary>
        /// Adds an admin employee based on the provided details.
        /// </summary>
        /// <param name="firstName">The first name of the employee.</param>
        /// <param name="lastName">The last name of the employee.</param>
        /// <param name="identityUserId">The identity user ID.</param>
        /// <returns>The ID of the newly created admin employee.</returns>
        public async Task<string> AddAdminEmployeeAsync(string firstName, string lastName, string identityUserId)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("First name and last name are required.");

            var employee = new Employee()
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

        public async Task<bool> SaveChangesAsync()
            => await employeeRepository.SaveAsync();

    }
}
