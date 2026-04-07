namespace ElProApp.Web.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using ElProApp.Web.Models.Admin;
    using ElProApp.Data;
    using ElProApp.Data.Models;
    using Interfaces;
    using ElProApp.Application.Services.Interfaces;

    using static ElProApp.Common.EntityValidationConstants.CalculationAction;

    /// <summary>
    /// Provides administrative operations such as managing user roles
    /// and restoring soft-deleted entities.
    /// </summary>
    public class AdminService(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ElProAppDbContext dbContext,
        SignInManager<IdentityUser> signInManager,
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider)
        : IAdminService
    {
        /// <summary>
        /// Retrieves the list of users and their assigned roles.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="UserRolesViewModel"/> containing user-role information.
        /// </returns>
        public async Task<List<UserRolesViewModel>> GetUsersRolesAsync()
        {
            var users = userManager.Users.ToList();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);

                userRolesViewModel.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user!.UserName!,
                    Roles = roles
                });
            }

            return userRolesViewModel;
        }

        /// <summary>
        /// Retrieves all available role names in the system.
        /// </summary>
        /// <returns>
        /// A collection of role names.
        /// </returns>
        public async Task<List<string?>> GetAllRolesAsync()
            => await roleManager.Roles
                .Select(r => r.Name)
                .ToListAsync();

        /// <summary>
        /// Assigns or removes roles for a specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roles">The roles to add or remove.</param>
        /// <param name="state">The operation type ("add" or "remove").</param>
        /// <returns>
        /// True if the operation was successful; otherwise, false.
        /// </returns>
        public async Task<bool> PostUsersRolesAsync(
            string userId,
            List<string> roles,
            string state)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            var currentRoles = await userManager.GetRolesAsync(user);

            switch (state)
            {
                case "add":
                    var rolesToAdd = roles.Except(currentRoles).ToList();
                    if (rolesToAdd.Any())
                        await userManager.AddToRolesAsync(user, rolesToAdd);
                    break;

                case "remove":
                    var rolesToRemove = roles.Intersect(currentRoles).ToList();
                    if (rolesToRemove.Any())
                        await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    break;
            }

            var currentUser = await userManager
                .GetUserAsync(httpContextAccessor.HttpContext.User);

            if (currentUser != null && currentUser.Id == user.Id)
                await signInManager.RefreshSignInAsync(user);

            return true;
        }

        /// <summary>
        /// Retrieves all soft-deleted entities of the specified type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>
        /// A collection of deleted entities.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the entity type does not contain an IsDeleted property.
        /// </exception>
        public async Task<List<T>> GetDeletedEntitiesAsync<T>() where T : class
        {
            var isDeletedProperty = typeof(T).GetProperty("IsDeleted");

            if (isDeletedProperty == null)
                throw new InvalidOperationException(
                    "The type does not contain the 'IsDeleted' property.");

            return await dbContext.Set<T>()
                .IgnoreQueryFilters()
                .Where(e => EF.Property<bool>(e, "IsDeleted") == true)
                .ToListAsync();
        }

        /// <summary>
        /// Restores a soft-deleted entity by identifier.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The entity identifier.</param>
        /// <returns>
        /// True if the entity was restored; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the provided identifier is invalid.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the entity does not contain required soft-delete properties.
        /// </exception>
        public async Task<bool> RestoreDeletedEntityAsync<T>(string id) where T : class
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId))
                throw new ArgumentException("Invalid ID format.");

            var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
            var deletedDateProperty = typeof(T).GetProperty("DeletedDate");

            if (isDeletedProperty == null || deletedDateProperty == null)
                throw new InvalidOperationException(
                    "The type does not contain the necessary properties for restoration.");

            var dbSet = dbContext.Set<T>();

            var entity = await dbSet
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == validId);

            if (entity == null)
                return false;

            isDeletedProperty.SetValue(entity, false);
            deletedDateProperty.SetValue(entity, null);

            if (entity is JobDone jobDone)
            {
                var jobDoneTeamMappingService =
                    serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();  
                
                var calculator =
                    serviceProvider.GetRequiredService<IEarningsCalculationService>();

                var helpMethodsService =
                    serviceProvider.GetRequiredService<IHelpMethodsService>();


                var mapping = await jobDoneTeamMappingService
                    .GetAllAttached()
                    .FirstOrDefaultAsync(x => x.JobDoneId == validId);

                if (mapping == null)
                    return false;

                var materials = await helpMethodsService.GetMaterialWhitQuantityAndPrice(jobDone.Materials , jobDone.BuildingId);

                await calculator.CalculateMoneyAsync(mapping.TeamId, jobDone.Id, jobDone.DaysForJob,materials, Adding);
            }

            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}