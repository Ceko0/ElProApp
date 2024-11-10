namespace ElProApp.Services.Data
{ 
    using Microsoft.EntityFrameworkCore;
    
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;

    public class EmployeeTeamMappingService(IRepository<EmployeeTeamMapping, Guid> _employeeTeamMappingRepository) : IEmployeeTeamMappingService
    {
        private readonly IRepository<EmployeeTeamMapping, Guid> employeeTeamMappingRepository = _employeeTeamMappingRepository;

        public async Task<ICollection<EmployeeTeamMapping>> GetAllAsync()
        {
            var entitys = await employeeTeamMappingRepository.GetAllAsync();

            var model = new List<EmployeeTeamMapping>(entitys);

            return model;
        }

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

        public async Task<EmployeeTeamMapping> AddAsync(Guid employeeId, Guid teamId)
        {
            if (employeeId == Guid.Empty ) throw new ArgumentNullException ("Invalid emlpoyeeId");
            if (teamId == Guid.Empty) throw new ArgumentNullException("Invalid teamId");

            var emlpoyeeTeamMappingEntity = new EmployeeTeamMapping()
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                TeamId = teamId
            };

            await employeeTeamMappingRepository.AddAsync(emlpoyeeTeamMappingEntity);

            return emlpoyeeTeamMappingEntity;
        } 

        public async Task<ICollection<EmployeeTeamMapping>> GetByTeamIdAsync(Guid id)
        {
            var entity = await employeeTeamMappingRepository
            .GetAllAttached()
            .Where(x => x.TeamId == id)
            .Include(x => x.Employee)
            .Include(x => x.Team)
            .ToListAsync();

            return entity;
        }
        
        private static Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId)) throw new ArgumentException("Invalid ID format.");
            return validId;
        }

        public bool Any(Guid employeeId, Guid teamId)
        {
            var model = employeeTeamMappingRepository
                .GetAllAttached().Where(x => x.EmployeeId == employeeId && x.TeamId == teamId);
            return model.Any();
        }

        public async Task<bool> RemoveAsync(EmployeeTeamMapping mapping) 
            => await employeeTeamMappingRepository.DeleteByCompositeKeyAsync(mapping.EmployeeId, mapping.TeamId);
        
    }
}
