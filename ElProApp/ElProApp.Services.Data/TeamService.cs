namespace ElProApp.Services.Data
{ 
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.Team;   

    public class TeamService : ITeamService
    {
        // Repository for handling Team data with generic ID type Guid
        private readonly IRepository<Team, Guid> teamRepository;

        // Constructor to initialize the repository
        public TeamService(IRepository<Team, Guid> repository)
        {
            teamRepository = repository;
        }

        /// <summary>
        /// Adds a new team based on the provided input model. 
        /// Throws an exception if a team with the same name already exists.
        /// </summary>
        /// <param name="model">TeamAddInputModel containing data for the new team.</param>
        /// <returns>ID of the newly created team as a string.</returns>
        public async Task<string> AddAsync(TeamAddInputModel model)
        {
            if ((await teamRepository.FirstOrDefaultAsync(x => x.Name == model.Name)) != null)
                throw new InvalidOperationException("A team with this name already exists!");

            var team = AutoMapperConfig.MapperInstance.Map<Team>(model);

            await teamRepository.AddAsync(team);
            return team.Id.ToString();
        }

        /// <summary>
        /// Retrieves a team by its ID for editing purposes. 
        /// Throws an exception if the team is marked as deleted.
        /// </summary>
        /// <param name="id">ID of the team as a string.</param>
        /// <returns>TeamEditInputModel mapped from the retrieved team.</returns>
        public async Task<TeamEditInputModel> EditByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            Team entity = await teamRepository.GetByIdAsync(validId);
            if (entity.IsDeleted) throw new InvalidOperationException("Team is deleted.");

            return AutoMapperConfig.MapperInstance.Map<TeamEditInputModel>(entity);
        }

        /// <summary>
        /// Updates a team entity based on the provided input model.
        /// Returns true if the update was successful, otherwise false.
        /// </summary>
        /// <param name="model">TeamEditInputModel containing updated team data.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        public async Task<bool> EditByModelAsync(TeamEditInputModel model)
        {
            try
            {
                var entity = await teamRepository.GetByIdAsync(model.id);
                AutoMapperConfig.MapperInstance.Map(model, entity);

                await teamRepository.SaveAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves all teams that are not marked as deleted.
        /// Maps them to TeamAllViewModel.
        /// </summary>
        /// <returns>IEnumerable of TeamAllViewModel.</returns>
        public async Task<IEnumerable<TeamAllViewModel>> GetAllAsync()
            => await teamRepository.GetAllAttached()
                                   .Where(x => !x.IsDeleted)
                                   .To<TeamAllViewModel>()
                                   .ToArrayAsync();

        /// <summary>
        /// Retrieves a specific team by its ID.
        /// Throws an exception if the team is not found.
        /// </summary>
        /// <param name="id">ID of the team as a string.</param>
        /// <returns>TeamViewModel mapped from the retrieved team.</returns>
        public async Task<TeamViewModel> GetByIdAsync(string id)
        {
            Guid validId = ConvertAndTestIdToGuid(id);
            var entity = await teamRepository.GetByIdAsync(validId).ConfigureAwait(false);
            return entity != null
                ? AutoMapperConfig.MapperInstance.Map<TeamViewModel>(entity)
                : throw new ArgumentException("Missing entity.");
        }

        /// <summary>
        /// Performs a soft delete of a team, marking it as deleted without removing it from the database.
        /// Returns true if the deletion was successful, otherwise false.
        /// </summary>
        /// <param name="id">ID of the team as a string.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        public async Task<bool> SoftDeleteAsync(string id)
        {
            try
            {
                Guid validId = ConvertAndTestIdToGuid(id);
                bool isDeleted = await teamRepository.SoftDeleteAsync(validId);
                return isDeleted;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a string ID to a GUID and verifies its validity.
        /// Throws an exception if the ID is null, empty, or invalid.
        /// </summary>
        /// <param name="id">ID of the team as a string.</param>
        /// <returns>Valid Guid ID.</returns>
        private static Guid ConvertAndTestIdToGuid(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId))
                throw new ArgumentException("Invalid ID format.");
            return validId;
        }
    }
}
