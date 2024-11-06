namespace ElProApp.Services.Data
{ 
    using Microsoft.EntityFrameworkCore;
    
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;

    public class EmployeeTeamMappingService(IRepository<EmployeeTeamMapping, Guid> _employeeTeamMappingRepository) : IEmployeeTeamMappingService
    {
        private readonly IRepository<EmployeeTeamMapping, Guid> employeeTeamMappingRepository = _employeeTeamMappingRepository;

        public async Task<IEnumerable<EmployeeTeamMapping>> GetAllAsync()
        {
            var entity = await employeeTeamMappingRepository.GetAllAsync();

            return entity;
        }

        public IEnumerable<EmployeeTeamMapping> GetAllByEmployeeId(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var entity = employeeTeamMappingRepository.GetAllAttached().Where(x => x.EmployeeId == validId).Include(x => x.Team);
            return entity;
        }       

        public async Task<EmployeeTeamMapping> AddAsync(Guid employeeId, Guid teamId)
        {
            if (employeeId == Guid.Empty ) throw new ArgumentNullException ("Invalid emlpoyeeId");
            if (teamId == Guid.Empty) throw new ArgumentNullException("Invalid teamId");

            var emlpoyeeTeamMappingEntity = new EmployeeTeamMapping()
            {
                EmployeeId = employeeId,
                TeamId = teamId
            };

            await employeeTeamMappingRepository.AddAsync(emlpoyeeTeamMappingEntity);

            return emlpoyeeTeamMappingEntity;
        }
               
        public IEnumerable<EmployeeTeamMapping> GetAllByTeamId(Guid id) => employeeTeamMappingRepository.GetAllAttached().Where(x => x.TeamId == id).Include(x => x.EmployeeId);
        
        
        private static Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId)) throw new ArgumentException("Invalid ID format.");
            return validId;
        }

    }
}
