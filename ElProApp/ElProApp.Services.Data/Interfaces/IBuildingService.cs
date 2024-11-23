namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Data.Models;
    using ElProApp.Web.Models.Building;

    public interface IBuildingService
    {
        /// <summary>
        /// Adds a new building to the system based on the provided input model.
        /// </summary>
        /// <param name="model">The model containing building data, including name, location, and other relevant details.</param>
        /// <returns>The unique ID of the newly added building as a string.</returns>
        Task<string> AddAsync(BuildingInputModel model);

        /// <summary>
        /// Retrieves detailed information about a building by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <returns>
        /// A <see cref="BuildingViewModel"/> containing the building's details, 
        /// or null if no building is found with the specified ID.
        /// </returns>
        Task<BuildingViewModel?> GetByIdAsync(string id);

        /// <summary>
        /// Retrieves the edit input model for a building by its unique ID.
        /// This model is typically used to populate a form for editing.
        /// </summary>
        /// <param name="id">The unique identifier of the building to be edited.</param>
        /// <returns>
        /// A <see cref="BuildingEditInputModel"/> containing the current data of the building.
        /// Throws an exception if the building does not exist.
        /// </returns>
        Task<BuildingEditInputModel> GetEditByIdAsync(string id);

        /// <summary>
        /// Updates the details of an existing building using the provided edit input model.
        /// </summary>
        /// <param name="model">The model containing the updated data for the building.</param>
        /// <returns>
        /// A boolean value indicating whether the update operation was successful.
        /// Returns false if the building does not exist or the update failed.
        /// </returns>
        Task<bool> EditByModelAsync(BuildingEditInputModel model);

        /// <summary>
        /// Retrieves a list of all buildings in the system.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="BuildingViewModel"/> objects representing all buildings.
        /// If no buildings exist, an empty collection is returned.
        /// </returns>
        Task<ICollection<BuildingViewModel>> GetAllAsync();

        /// <summary>
        /// Retrieves all buildings as a queryable collection, including related entities such as employees or projects.
        /// Useful for cases where deferred execution or filtering is needed.
        /// </summary>
        /// <returns>
        /// A queryable collection of <see cref="Building"/> objects.
        /// Ensure that proper query handling is applied when accessing related entities.
        /// </returns>
        IQueryable<Building> GetAllAttached();

        /// <summary>
        /// Soft deletes a building from the system by marking it as inactive.
        /// </summary>
        /// <param name="id">The unique identifier of the building to delete.</param>
        /// <returns>
        /// A boolean value indicating whether the deletion was successful.
        /// Returns false if the building does not exist or deletion failed.
        /// </returns>
        Task<bool> SoftDeleteAsync(string id);
    }
}
