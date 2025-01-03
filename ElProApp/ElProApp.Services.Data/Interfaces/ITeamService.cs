namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models;
    using ElProApp.Web.Models.Team;

    public interface ITeamService
    {
        /// <summary>
        /// Creates a default input model for adding a new team.
        /// </summary>
        /// <returns>
        /// A default <see cref="TeamInputModel"/> pre-populated with default values for a new team.
        /// </returns>
        Task<TeamInputModel> AddAsync();

        /// <summary>
        /// Adds a new team based on the provided input model.
        /// </summary>
        /// <param name="model">
        /// The <see cref="TeamInputModel"/> containing the details for the new team.
        /// </param>
        /// <returns>
        /// The unique ID of the newly added team as a string.
        /// </returns>
        Task<string> AddAsync(TeamInputModel model);

        /// <summary>
        /// Retrieves a team by its unique ID.
        /// </summary>
        /// <param name="id">
        /// The unique ID of the team.
        /// </param>
        /// <returns>
        /// A <see cref="TeamViewModel"/> representing the requested team. Returns null if not found.
        /// </returns>
        Task<TeamViewModel> GetByIdAsync(string id);

        /// <summary>
        /// Retrieves all Teams as a queryable collection, including related entities for complex querying.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> of <see cref="Team"/> entities.
        /// </returns>
        IQueryable<Team> GetAllAttached();

        /// <summary>
        /// Retrieves the edit model for a team by its unique ID.
        /// </summary>
        /// <param name="id">
        /// The unique ID of the team.
        /// </param>
        /// <returns>
        /// A <see cref="TeamEditInputModel"/> containing the editable details of the team.
        /// </returns>
        Task<TeamEditInputModel> EditByIdAsync(string id);

        /// <summary>
        /// Updates an existing team using the provided edit input model.
        /// </summary>
        /// <param name="model">
        /// The <see cref="TeamEditInputModel"/> containing updated details of the team.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the update operation was successful.
        /// </returns>
        Task<bool> EditByModelAsync(TeamEditInputModel model);

        /// <summary>
        /// Retrieves all Teams as a list for simplified data consumption.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="TeamViewModel"/> representing all Teams.
        /// </returns>
        Task<ICollection<TeamViewModel>> GetAllAsync();

        /// <summary>
        /// Soft deletes a team by marking it as inactive rather than permanently removing it.
        /// </summary>
        /// <param name="id">
        /// The unique ID of the team.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the soft delete operation was successful.
        /// </returns>
        Task<bool> SoftDeleteAsync(string id);

        /// <summary>
        /// Checks if a team exists by its unique ID.
        /// </summary>
        /// <param name="id">
        /// The unique ID of the team.
        /// </param>
        /// <returns>
        /// A boolean value. Returns true if the team exists, otherwise false.
        /// </returns>
        Task<bool> Any(Guid id);
    }
}
