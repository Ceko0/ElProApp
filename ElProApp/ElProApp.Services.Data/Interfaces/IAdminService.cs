namespace ElProApp.Services.Data.Interfaces
{
    using ElProApp.Web.Models.Admin;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;

    /// <summary>
    /// Defines the methods for managing user roles and deleted entities in the admin section.
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        /// Gets a list of users and their roles.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="UserRolesViewModel"/>.</returns>
        public Task<List<UserRolesViewModel>> GetUsersRolesAsync();

        /// <summary>
        /// Gets a list of all available roles in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of role names as <see cref="List{string}"/>.</returns>
        public Task<List<string?>> GetAllRolesAsync();

        /// <summary>
        /// Posts user roles to the system by adding or removing roles based on the specified action.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roles">The list of roles to be assigned or removed.</param>
        /// <param name="state">The action to be performed: "add" or "remove".</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="bool"/> indicating success or failure.</returns>
        public Task<bool> PostUsersRolesAsync(string userId, List<string> roles, string state);

        /// <summary>
        /// Retrieves a list of deleted entities.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve. It must have the <see cref="IsDeleted"/> property.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IQueryable{T}"/> of deleted entities.</returns>
        public IQueryable<T> GetDeletedEntities<T>() where T : class;

        /// <summary>
        /// Restores a deleted entity back to the system.
        /// </summary>
        /// <typeparam name="T">The type of entity to restore. It must have the <see cref="IsDeleted"/> property.</typeparam>
        /// <param name="id">The identifier of the entity to restore.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="bool"/> indicating whether the entity was restored successfully.</returns>
        public Task<bool> RestoreDeletedEntityAsync<T>(string id) where T : class;
    }
}
