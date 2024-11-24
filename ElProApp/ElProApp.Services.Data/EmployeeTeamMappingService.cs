 namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Service class for managing employee-team mapping operations, including adding, removing, and retrieving mappings.
    /// </summary>
    public class EmployeeTeamMappingService(IRepository<EmployeeTeamMapping, Guid> _employeeTeamMappingRepository,IServiceProvider _serviceProvider)
        : IEmployeeTeamMappingService
    {
        private readonly IRepository<EmployeeTeamMapping, Guid> employeeTeamMappingRepository = _employeeTeamMappingRepository;
        private readonly IServiceProvider serviceProvider = _serviceProvider;

        /// <summary>
        /// Retrieves all employee-team mappings, including the associated employee and team details.
        /// </summary>
        /// <returns>A collection of <see cref="EmployeeTeamMapping"/> objects.</returns>
        public async Task<ICollection<EmployeeTeamMapping>> GetAllAsync()
            => await employeeTeamMappingRepository
            .GetAllAttached()
            .Include(x => x.Employee)
            .Include(x => x.Team)
            .ToListAsync();

        /// <summary>
        /// Retrieves all employee-team mappings, including attached entities.
        /// </summary>
        /// <returns>An <see cref="IQueryable"/> collection of <see cref="EmployeeTeamMapping"/> objects.</returns>
        public IQueryable<EmployeeTeamMapping> GetAllAttached()
            => employeeTeamMappingRepository
            .GetAllAttached();

        /// <summary>
        /// Retrieves all employee-team mappings for a specific employee by their ID.
        /// </summary>
        /// <param name="id">The ID of the employee to retrieve the mappings for.</param>
        /// <returns>A collection of <see cref="EmployeeTeamMapping"/> objects associated with the given employee.</returns>
        public ICollection<EmployeeTeamMapping> GetAllByEmployeeId(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);

            var entity = employeeTeamMappingRepository
                .GetAllAttached()
                .Where(x => x.EmployeeId == validId)
                .Include(x => x.Team)
                .ToList();

            return entity;
        }

        /// <summary>
        /// Adds a new employee-team mapping.
        /// </summary>
        /// <param name="employeeId">The ID of the employee to map.</param>
        /// <param name="teamId">The ID of the team to map to the employee.</param>
        /// <returns>The created <see cref="EmployeeTeamMapping"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the employeeId or teamId is invalid (empty GUID).</exception>
        public async Task<EmployeeTeamMapping> AddAsync(Guid employeeId, Guid teamId)
        {
            if (employeeId == Guid.Empty) throw new ArgumentNullException("Invalid employeeId");
            if (teamId == Guid.Empty) throw new ArgumentNullException("Invalid teamId");

            var employeeService = serviceProvider.GetService<IEmployeeService>()!;
            var teamService = serviceProvider.GetService<ITeamService>()!;

            var employeeExists = await employeeService.GetAllAttached().FirstOrDefaultAsync(b => b.Id == employeeId && !b.IsDeleted);
            if (employeeExists == null) throw new InvalidOperationException("Employee not found");

            var teamExists = await teamService.GetAllAttached().FirstOrDefaultAsync(t => t.Id == teamId && !t.IsDeleted);
            if (teamExists == null) throw new InvalidOperationException("Team not found");            

            var employeeTeamMappingEntity = new EmployeeTeamMapping()
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                TeamId = teamId
            };

            await employeeTeamMappingRepository.AddAsync(employeeTeamMappingEntity);

            return employeeTeamMappingEntity;
        }

        /// <summary>
        /// Retrieves all employee-team mappings associated with a specific team by its ID.
        /// </summary>
        /// <param name="id">The ID of the team to retrieve the mappings for.</param>
        /// <returns>A collection of <see cref="EmployeeTeamMapping"/> objects associated with the given team.</returns>
        public async Task<ICollection<EmployeeTeamMapping>> GetByTeamIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("TeamId must not be empty.");

            var entity = await employeeTeamMappingRepository
            .GetAllAttached()
            .Where(x => x.TeamId == id)
            .Include(x => x.Employee)
            .Include(x => x.Team)
            .ToListAsync();

            return entity;
        }

        /// <summary>
        /// Checks if a specific employee is already mapped to a specific team.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="teamId">The ID of the team.</param>
        /// <returns>True if the employee is already mapped to the team; otherwise, False.</returns>
        public bool Any(Guid employeeId, Guid teamId)
        {
            if (employeeId == Guid.Empty || teamId == Guid.Empty)
                throw new ArgumentException("EmployeeId and TeamId must not be empty.");

            var model = employeeTeamMappingRepository
                .GetAllAttached()
                .Where(x => x.EmployeeId == employeeId && x.TeamId == teamId);

            return model.Any();
        }

        /// <summary>
        /// Removes an employee-team mapping.
        /// </summary>
        /// <param name="mapping">The <see cref="EmployeeTeamMapping"/> object to remove.</param>
        /// <returns>True if the removal was successful; otherwise, False.</returns>
        public async Task<bool> RemoveAsync(EmployeeTeamMapping mapping)
        {
            // Проверка дали съществува връзка с този EmployeeId и TeamId
            var mappingExists = await employeeTeamMappingRepository
                .GetAllAttached()
                .AnyAsync(x => x.EmployeeId == mapping.EmployeeId && x.TeamId == mapping.TeamId);

            if (!mappingExists) throw new InvalidOperationException("Mapping not found.");

            return await employeeTeamMappingRepository.DeleteByCompositeKeyAsync(mapping.EmployeeId, mapping.TeamId);
        }

        /// <summary>
        /// Converts and validates a string ID to a valid <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">The string ID to convert and validate.</param>
        /// <returns>A valid <see cref="Guid"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the ID format is invalid.</exception>
        private static Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId))
                throw new ArgumentException("Invalid ID format.");
            return validId;
        }
    }
}
