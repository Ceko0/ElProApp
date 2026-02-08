namespace ElProApp.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Application.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing employee-team mappings.
    /// </summary>
    public class EmployeeTeamMappingService : IEmployeeTeamMappingService
    {
        private readonly IRepository<EmployeeTeamMapping, object> employeeTeamMappingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeTeamMappingService"/> class.
        /// </summary>
        public EmployeeTeamMappingService(
            IRepository<EmployeeTeamMapping, object> employeeTeamMappingRepository)
        {
            this.employeeTeamMappingRepository = employeeTeamMappingRepository;
        }

        /// <summary>
        /// Retrieves all employee-team mappings including related employee and team entities.
        /// </summary>
        public async Task<ICollection<EmployeeTeamMapping>> GetAllAsync()
            => await employeeTeamMappingRepository
                .GetAllAttached()
                .Include(x => x.Employee)
                .Include(x => x.Team)
                .ToListAsync();

        /// <summary>
        /// Returns all employee-team mappings as an attached query.
        /// </summary>
        public IQueryable<EmployeeTeamMapping> GetAllAttached()
            => employeeTeamMappingRepository.GetAllAttached();

        /// <summary>
        /// Retrieves all mappings for a given employee identifier.
        /// </summary>
        public ICollection<EmployeeTeamMapping> GetAllByEmployeeId(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);

            return employeeTeamMappingRepository
                .GetAllAttached()
                .Include(x => x.Team)
                .Where(x =>
                    x.EmployeeId == validId &&
                    !x.Team.IsDeleted)
                .ToList();
        }

        /// <summary>
        /// Creates a new employee-team mapping.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when employeeId or teamId is empty.
        /// </exception>
        public async Task<EmployeeTeamMapping> AddAsync(Guid employeeId, Guid teamId)
        {
            if (employeeId == Guid.Empty)
                throw new ArgumentException(
                    "EmployeeId must not be empty.", nameof(employeeId));

            if (teamId == Guid.Empty)
                throw new ArgumentException(
                    "TeamId must not be empty.", nameof(teamId));

            var mapping = new EmployeeTeamMapping
            {
                EmployeeId = employeeId,
                TeamId = teamId
            };

            await employeeTeamMappingRepository.AddAsync(mapping);
            return mapping;
        }

        /// <summary>
        /// Retrieves all mappings for a given team identifier.
        /// </summary>
        public async Task<ICollection<EmployeeTeamMapping>> GetByTeamIdAsync(Guid teamId)
        {
            if (teamId == Guid.Empty)
                throw new ArgumentException(
                    "TeamId must not be empty.", nameof(teamId));

            return await employeeTeamMappingRepository
                .GetAllAttached()
                .Where(x => x.TeamId == teamId)
                .Include(x => x.Employee)
                .Include(x => x.Team)
                .ToListAsync();
        }

        /// <summary>
        /// Determines whether a mapping exists for the specified employee and team.
        /// </summary>
        public bool Any(Guid employeeId, Guid teamId)
        {
            if (employeeId == Guid.Empty)
                throw new ArgumentException(
                    "EmployeeId must not be empty.", nameof(employeeId));

            if (teamId == Guid.Empty)
                throw new ArgumentException(
                    "TeamId must not be empty.", nameof(teamId));

            return employeeTeamMappingRepository
                .GetAllAttached()
                .Any(x =>
                    x.EmployeeId == employeeId &&
                    x.TeamId == teamId);
        }

        /// <summary>
        /// Removes an existing employee-team mapping.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the mapping does not exist.
        /// </exception>
        public async Task<bool> RemoveAsync(EmployeeTeamMapping mapping)
        {
            ArgumentNullException.ThrowIfNull(mapping);

            bool exists = await employeeTeamMappingRepository
                .GetAllAttached()
                .AnyAsync(x =>
                    x.EmployeeId == mapping.EmployeeId &&
                    x.TeamId == mapping.TeamId);

            if (!exists)
                throw new InvalidOperationException(
                    "Employee-team mapping not found.");

            return await employeeTeamMappingRepository
                .DeleteByCompositeKeyAsync(
                    mapping.EmployeeId,
                    mapping.TeamId);
        }

        private static Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrWhiteSpace(id) ||
                !Guid.TryParse(id, out Guid validId))
            {
                throw new ArgumentException(
                    "Invalid ID format.", nameof(id));
            }

            return validId;
        }
    }
}
