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
    using ElProApp.Web.Models.JobDone;
    using ElProApp.Data.Models;
    using Interfaces;
    using ElProApp.Application.Services.Interfaces;
    using static ElProApp.Common.EntityValidationConstants.CalculationAction;

    public class AdminService(UserManager<IdentityUser> userManager,
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
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="UserRolesViewModel"/> objects.</returns>
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
        /// Retrieves a list of all roles in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of role names.</returns>
        public async Task<List<string?>> GetAllRolesAsync()
            => await roleManager.Roles.Select(r => r.Name)
            .ToListAsync();

        /// <summary>
        /// Assigns or removes roles for a specified user based on the given state.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="roles">The list of roles to assign or remove.</param>
        /// <param name="state">The action to be performed ("add" or "remove").</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the operation was successful.</returns>
        /// <summary>
        /// Assigns or removes roles for a specified user.
        /// </summary>
        public async Task<bool> PostUsersRolesAsync(string userId, List<string> roles, string state)
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

            var currentUser = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);

            if (currentUser != null && currentUser.Id == user.Id)
                await signInManager.RefreshSignInAsync(user);

            return true;
        }

        /// <summary>
        /// Retrieves deleted entities ignoring global query filters.
        /// </summary>
        public async Task<List<T>> GetDeletedEntitiesAsync<T>() where T : class
        {
            var isDeletedProperty = typeof(T).GetProperty("IsDeleted");

            if (isDeletedProperty == null)
                throw new InvalidOperationException("The type does not contain the 'IsDeleted' property.");

            return await dbContext.Set<T>()
                .IgnoreQueryFilters()
                .Where(e => EF.Property<bool>(e, "IsDeleted") == true)
                .ToListAsync();
        }

        /// <summary>
        /// Restores a deleted entity by setting the 'IsDeleted' property to false and the 'DeletedDate' property to null.
        /// </summary>
        /// <typeparam name="T">The type of entity to restore. The type must contain the 'IsDeleted' and 'DeletedDate' properties.</typeparam>
        /// <param name="id">The unique identifier of the entity to restore.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the restoration was successful.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided ID is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the entity type does not contain the necessary properties for restoration.</exception>
        public async Task<bool> RestoreDeletedEntityAsync<T>(string id) where T : class
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid validId))
                throw new ArgumentException("Invalid ID format.");

            var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
            var deletedDateProperty = typeof(T).GetProperty("DeletedDate");

            if (isDeletedProperty == null || deletedDateProperty == null)
                throw new InvalidOperationException("The type does not contain the necessary properties for restoration.");

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
                JobDoneInputModel model = new()
                {
                    Id = jobDone.Id,
                    BuildingId = jobDone.BuildingId,
                    DaysForJob = jobDone.DaysForJob,
                    Jobs = new()
                };

                var jboDoneTeamMappingService = serviceProvider.GetRequiredService<IJobDoneTeamMappingService>();

                var jobDoneTeamMapping = await jboDoneTeamMappingService.GetAllAttached()
                    .FirstOrDefaultAsync(x => x.JobDoneId == validId);

                if (jobDoneTeamMapping == null)
                    return false;

                model.TeamId = jobDoneTeamMapping.TeamId;
                var jobs = await serviceProvider.GetRequiredService<IJobDoneJobMappingService>()
                    .GetAllAttached().Where(x => x.JobDoneId == jobDone.Id)
                    .ToDictionaryAsync(x => x.JobId, x => x.Quantity);

                var calculator = serviceProvider.GetRequiredService<IEarningsCalculationService>();
                await calculator.CalculateMoneyAsync(jobDoneTeamMapping.TeamId, jobs, jobDone.Id, jobDone.DaysForJob, Add);
            }

            await dbContext.SaveChangesAsync();

            return true;
        }

    }
}
