namespace ElProAppServices.Test
{
    using Moq;
    using NUnit.Framework;
    using MockQueryable;

    using System;
    using System.Security.Claims;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Http;

    using ElProApp.Services.Data;
    using ElProApp.Data;

    [TestFixture]
    public class AdminServiceTests : ConstructorForMock
    {
        private Mock<UserManager<IdentityUser>> mockUserManager;
        private Mock<RoleManager<IdentityRole>> mockRoleManager;
        private ElProAppDbContext dbContext;
        private SignInManager<IdentityUser> signInManager;
        private Mock<IHttpContextAccessor> mockHttpContextAccessor;
        private AdminService _adminService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ElProAppDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDb")
                .Options;

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(new ElProAppDbContext(options));

            mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var mockUserClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();

            signInManager = new SignInManager<IdentityUser>(
                mockUserManager.Object,
                mockHttpContextAccessor.Object,
                mockUserClaimsPrincipalFactory.Object,
                null,
                null,
                null

            );

            _adminService = new AdminService(
                mockUserManager.Object,
                mockRoleManager.Object,
                dbContext,
                signInManager,
                mockHttpContextAccessor.Object
            );
        }

        [Test]
        public async Task GetUsersRolesAsync_ShouldReturnListOfUserRolesViewModel()
        {
            var mockUser = new IdentityUser { Id = "1", UserName = "testuser" };
            var mockRole = new List<string> { "Admin", "User" };

            IQueryable<IdentityUser> queryableIdentityUsers = IdentityUsers.BuildMock();
            mockUserManager.Setup(um => um.Users).Returns(queryableIdentityUsers);
            mockUserManager.Setup(um => um.GetRolesAsync(mockUser)).ReturnsAsync(mockRole);

            var result = await _adminService.GetUsersRolesAsync();

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task PostUsersRolesAsync_ShouldAddRolesSuccessfully()
        {
            var userId = "1";
            var rolesToAdd = new List<string> { "Admin", "Manager" };
            var mockUser = new IdentityUser { Id = userId, UserName = "testuser" };

            mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(mockUser);
            mockUserManager.Setup(um => um.GetRolesAsync(mockUser)).ReturnsAsync(new List<string> { "User" });
            mockUserManager.Setup(um => um.AddToRolesAsync(mockUser, rolesToAdd)).ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(identityUser1);
            mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());
            var result = await _adminService.PostUsersRolesAsync(userId, rolesToAdd, "add");

            Assert.IsTrue(result);
            mockUserManager.Verify(um => um.AddToRolesAsync(mockUser, rolesToAdd), Times.Once);
        }

        [Test]
        public async Task PostUsersRolesAsync_ShouldRemoveRolesSuccessfully()
        {
            var userId = "1";
            var rolesToRemove = new List<string> { "User" };
            var mockUser = new IdentityUser { Id = userId, UserName = "testuser" };

            mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(mockUser);
            mockUserManager.Setup(um => um.GetRolesAsync(mockUser)).ReturnsAsync(new List<string> { "User", "Admin" });
            mockUserManager.Setup(um => um.RemoveFromRolesAsync(mockUser, rolesToRemove)).ReturnsAsync(IdentityResult.Success);
            mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());
            var result = await _adminService.PostUsersRolesAsync(userId, rolesToRemove, "remove");

            Assert.IsTrue(result);
            mockUserManager.Verify(um => um.RemoveFromRolesAsync(mockUser, rolesToRemove), Times.Once);
        }

        [Test]
        public async Task RestoreDeletedEntityAsync_ShouldThrowArgumentException_WhenPropertyTypeIsInvalid()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => _adminService.RestoreDeletedEntityAsync<object>("3f11c6e3-ec7b-4c56-b2f4-ecf77c3d4b88"));
        }

        [Test]
        public void GetDeletedEntities_ShouldThrowException_WhenEntityDoesNotContainIsDeletedProperty()
        {
            var invalidEntity = new { Id = "1", Name = "Invalid Entity" };

            Assert.Throws<InvalidOperationException>(() => _adminService.GetDeletedEntities<object>());
        }

        [Test]
        public void RestoreDeletedEntityAsync_ShouldThrowArgumentException_WhenInvalidId()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _adminService.RestoreDeletedEntityAsync<object>("InvalidId"));
        }
    }
}
