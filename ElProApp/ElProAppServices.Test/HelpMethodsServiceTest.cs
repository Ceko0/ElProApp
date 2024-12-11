namespace ElProAppServices.Test
{
    using Moq;
    using NUnit.Framework;
    using MockQueryable;

    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;

    using ElProApp.Services.Data;
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Data.Models;
    using ElProApp.Services.Data.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    public class HelpMethodsServiceTests : ConstructorForMock
    {
        private HelpMethodsService helpMethodsService;
        private Mock<IHttpContextAccessor> mockHttpContextAccessor;
        private Mock<UserManager<IdentityUser>> mockUserManager;

        [SetUp]
        public void SetUp()
        {
            mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            mockUserManager = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            helpMethodsService = new HelpMethodsService(
                mockHttpContextAccessor.Object,
                mockUserManager.Object,
                mockServiceProvider.Object
            );
        }

        [Test]
        public void GetUserId_ShouldThrowExceptionIfUserIdIsNull()
        {
            var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }));
            mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(mockClaimsPrincipal);

            Assert.Throws<InvalidOperationException>(() => helpMethodsService.GetUserId());
        }

        [Test]
        public async Task GetUserAsync_ShouldReturnUser()
        {
            var userId = "test-user-id";
            var mockUser = new IdentityUser { Id = userId };
            mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(mockUser);

            var result = await helpMethodsService.GetUserAsync(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(mockUser, result);
        }

        [Test]
        public void GetUserAsync_ShouldThrowExceptionIfUserNotFound()
        {
            var userId = "non-existent-user-id";
            mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((IdentityUser)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await helpMethodsService.GetUserAsync(userId));
        }

        [Test]
        public void ConvertAndTestIdToGuid_ShouldReturnValidGuid()
        {
            var validId = "d4d3e6fb-6a7b-48d4-b8f7-b16b9e5290a5";

            var result = helpMethodsService.ConvertAndTestIdToGuid(validId);

            Assert.AreEqual(Guid.Parse(validId), result);
        }

        [Test]
        public void ConvertAndTestIdToGuid_ShouldThrowArgumentExceptionIfInvalidGuid()
        {
            var invalidId = "invalid-id";

            Assert.Throws<ArgumentException>(() => helpMethodsService.ConvertAndTestIdToGuid(invalidId));
        }

        [Test]
        public async Task GetBuildingTeamMapping_ShouldReturnEmptyList_ForNonExistingBuildingId()
        {
            IQueryable<BuildingTeamMapping> queryableBuildingTeamMappings = buildingTeamMappings.BuildMock();
            mockBuildingTeamMappingService.Setup(x => x.GetAllAttached()).Returns(queryableBuildingTeamMappings);

            var result = await helpMethodsService.GetBuildingTeamMapping(buildingTeamMapping1.TeamId);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task GetTeamInformation_ShouldReturnCorrectTeam_ForValidJobDoneId()
        {
            IQueryable<JobDoneTeamMapping> queryableJobDoneTeamMappings = jobDoneTeamMappings.BuildMock();

            mockJobDoneTeamMappingService.Setup(x => x.GetAllAttached()).Returns(queryableJobDoneTeamMappings);

            var result = await helpMethodsService.GetTeamInforamtion(jobDoneTeamMapping1.JobDoneId);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetTeamInformation_ShouldReturnEmptyTeam_ForInvalidJobDoneId()
        {
            IQueryable<JobDoneTeamMapping> queryableJobDoneTeamMappings = jobDoneTeamMappings.BuildMock();

            mockJobDoneTeamMappingService.Setup(x => x.GetAllAttached()).Returns(queryableJobDoneTeamMappings);

            var result = await helpMethodsService.GetTeamInforamtion(jobDoneTeamMapping1.TeamId);

            Assert.IsNotNull(result);
            Assert.That(result, Is.TypeOf<Team>());
        }

        [Test]
        public async Task GetCurrentUserAsync_ShouldReturnCurrentUser_ForLoggedInUser()
        {
            var userId = "test-user-id";
            var mockUser = new IdentityUser { Id = userId };

            var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            { new Claim(ClaimTypes.NameIdentifier, userId)}));

            mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(mockClaimsPrincipal);
            mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(mockUser);

            var result = await helpMethodsService.GetCurrentUserAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(mockUser.Id, result.Id);
        }

        [Test]
        public async Task GetCurrentUserAsync_ShouldThrowException_IfUserNotLoggedIn()
        {
            var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { }));
            mockHttpContextAccessor.Setup(x => x.HttpContext.User).Returns(mockClaimsPrincipal);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await helpMethodsService.GetCurrentUserAsync());
        }

        [Test]
        public void GetAllJobDoneTeamMappings_ShouldReturnAllMappings()
        {
            IQueryable<JobDoneTeamMapping> queryableJobDoneTeamMappings = jobDoneTeamMappings.BuildMock();
            mockJobDoneTeamMappingService.Setup(x => x.GetAllAttached()).Returns(queryableJobDoneTeamMappings);

            var result = helpMethodsService.GetAllJobDoneTeamMappings();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetAllJobs_ShouldReturnAllNonDeletedJobs()
        {
            job1.IsDeleted = true;

            IQueryable<Job> queryableJobs = jobs.BuildMock();

            mockJobService.Setup(x => x.GetAllAttached()).Returns(queryableJobs);
      
            var result = helpMethodsService.GetAllJobs();

            Assert.That(result.Count(), Is.EqualTo(1)); 
            Assert.IsFalse(result.Any(x => x.IsDeleted)); 
        }

        [Test]
        public void GetAllEmployees_ShouldReturnAllNonDeletedEmployees()
        {
            employee1.IsDeleted = true;

            IQueryable<Employee> queryableEmployees = employees.BuildMock();

            mockEmployeeService.Setup(x => x.GetAllAttached()).Returns(queryableEmployees);

            var result = helpMethodsService.GetAllEmployees();

            Assert.That(result.Count(), Is.EqualTo(1)); 
            Assert.IsFalse(result.Any(x => x.IsDeleted)); 
        }

    }
}


