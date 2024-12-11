namespace ElProAppServices.Test
{
    using Moq;
    using NUnit.Framework;
    using MockQueryable;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using ElProApp.Data.Models;
    using ElProApp.Web.Models.Team;
    using ElProApp.Data.Models.Mappings;

    [TestFixture]
    public class TeamServiceTests : ConstructorForMock
    {
        [Test]
        public async Task AddAsync_ShouldReturnTeamInputModel()
        {
            IQueryable<Building> queryableBuildings = Buildings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllBuildings()).Returns(queryableBuildings);

            IQueryable<JobDone> queryableJobDones = jobDones.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllJobDones()).Returns(queryableJobDones);

            IQueryable<Employee> queryableEmployees = employees.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllEmployees()).Returns(queryableEmployees);

            var result = await teamService.AddAsync();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<TeamInputModel>(result);
        }

        [Test]
        public async Task EditByIdAsync_ShouldReturnTeamEditInputModel()
        {
            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(team1.Id.ToString())).Returns(team1.Id);

            mockHelpMethodsService.Setup(repo => repo.GetUserId()).Returns(identityUser1.Id);
            
            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(identityUser1.Id))
                .Returns(Guid.Parse(identityUser1.Id));

            IQueryable<EmployeeTeamMapping> queryableEmployeeTeamMappings = employeeTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllEmployeeTeamMаppings()).Returns(queryableEmployeeTeamMappings);
            mockEmployeeTeamMappingService.Setup(repo => repo.GetAllAttached()).Returns(queryableEmployeeTeamMappings);

            mockHelpMethodsService.Setup(repo => repo.GetUserAsync(identityUser1.Id)).ReturnsAsync(identityUser1);
            List<string> roles = new() { "Admin" };
            mockHelpMethodsService.Setup(repo => repo.GetUserRolesAsync(identityUser1)).ReturnsAsync(roles);

            IQueryable<Building> queryableBuildings = Buildings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllBuildings()).Returns(queryableBuildings);
            IQueryable<JobDone> queryableJobDones = jobDones.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllJobDones()).Returns(queryableJobDones);
            IQueryable<Employee> queryableEmployees = employees.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllEmployees()).Returns(queryableEmployees);
            IQueryable<BuildingTeamMapping> queryableBuildingTeamMappings = buildingTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllBuildingTeamMappings()).Returns(queryableBuildingTeamMappings);
            IQueryable<JobDoneTeamMapping> queryableJobDoneTeamMappings = jobDoneTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllJobDoneTeamMappings()).Returns(queryableJobDoneTeamMappings);

            mockTeamRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(team1);
            mockServiceProvider.Setup(sp => sp.GetService(It.IsAny<Type>())).Returns(mockServiceProvider.Object);

            var result = await teamService.EditByIdAsync(team1.Id.ToString());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<TeamEditInputModel>(result);
        }

        [Test]
        public async Task EditByModelAsync_ShouldReturnTrueOnSuccess()
        {
            var model = new TeamEditInputModel { Id = team1.Id };

            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(team1.Id.ToString())).Returns(team1.Id);

            mockHelpMethodsService.Setup(repo => repo.GetUserId()).Returns(identityUser1.Id);

            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(identityUser1.Id))
                .Returns(Guid.Parse(identityUser1.Id));

            IQueryable<EmployeeTeamMapping> queryableEmployeeTeamMappings = employeeTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllEmployeeTeamMаppings()).Returns(queryableEmployeeTeamMappings);

            mockHelpMethodsService.Setup(repo => repo.GetUserAsync(identityUser1.Id)).ReturnsAsync(identityUser1);
            List<string> roles = new() { "Admin" };
            mockHelpMethodsService.Setup(repo => repo.GetUserRolesAsync(identityUser1)).ReturnsAsync(roles);

            IQueryable<Building> queryableBuildings = Buildings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllBuildings()).Returns(queryableBuildings);
            IQueryable<JobDone> queryableJobDones = jobDones.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllJobDones()).Returns(queryableJobDones);
            IQueryable<Employee> queryableEmployees = employees.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllEmployees()).Returns(queryableEmployees);
            IQueryable<BuildingTeamMapping> queryableBuildingTeamMappings = buildingTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllBuildingTeamMappings()).Returns(queryableBuildingTeamMappings);
            IQueryable<JobDoneTeamMapping> queryableJobDoneTeamMappings = jobDoneTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllJobDoneTeamMappings()).Returns(queryableJobDoneTeamMappings);

            mockTeamRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(team1);

            var result = await teamService.EditByModelAsync(model);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task EditByModelAsync_ShouldReturnFalseOnFailure()
        {
            var model = new TeamEditInputModel { Id = Guid.NewGuid() };

            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(team1.Id.ToString())).Returns(team1.Id);

            mockHelpMethodsService.Setup(repo => repo.GetUserId()).Returns(identityUser1.Id);

            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(identityUser1.Id))
                .Returns(Guid.Parse(identityUser1.Id));

            IQueryable<EmployeeTeamMapping> queryableEmployeeTeamMappings = employeeTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllEmployeeTeamMаppings()).Returns(queryableEmployeeTeamMappings);

            mockHelpMethodsService.Setup(repo => repo.GetUserAsync(identityUser1.Id)).ReturnsAsync(identityUser1);
            List<string> roles = new() { "Admin" };
            mockHelpMethodsService.Setup(repo => repo.GetUserRolesAsync(identityUser1)).ReturnsAsync(roles);

            IQueryable<Building> queryableBuildings = Buildings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllBuildings()).Returns(queryableBuildings);
            IQueryable<JobDone> queryableJobDones = jobDones.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllJobDones()).Returns(queryableJobDones);
            IQueryable<Employee> queryableEmployees = employees.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllEmployees()).Returns(queryableEmployees);
            IQueryable<BuildingTeamMapping> queryableBuildingTeamMappings = buildingTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllBuildingTeamMappings()).Returns(queryableBuildingTeamMappings);
            IQueryable<JobDoneTeamMapping> queryableJobDoneTeamMappings = jobDoneTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllJobDoneTeamMappings()).Returns(queryableJobDoneTeamMappings);

            mockTeamRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(team1);
            mockTeamRepository.Setup(repo => repo.SaveAsync()).ThrowsAsync(new Exception());

            var result = await teamService.EditByModelAsync(model);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldReturnTrueOnSuccess()
        {
            var teamId = Guid.NewGuid().ToString();

            mockTeamRepository.Setup(repo => repo.SoftDeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await teamService.SoftDeleteAsync(teamId);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldReturnFalseOnFailure()
        {
            var teamId = Guid.NewGuid().ToString();

            mockTeamRepository.Setup(repo => repo.SoftDeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await teamService.SoftDeleteAsync(teamId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnTeamViewModel()
        {
            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(team1.Id.ToString())).Returns(team1.Id);

            mockHelpMethodsService.Setup(repo => repo.GetUserId()).Returns(identityUser1.Id);

            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(identityUser1.Id))
                .Returns(Guid.Parse(identityUser1.Id));

            IQueryable<EmployeeTeamMapping> queryableEmployeeTeamMappings = employeeTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllEmployeeTeamMаppings()).Returns(queryableEmployeeTeamMappings);

            mockHelpMethodsService.Setup(repo => repo.GetUserAsync(identityUser1.Id)).ReturnsAsync(identityUser1);
            List<string> roles = new() { "Admin" };
            mockHelpMethodsService.Setup(repo => repo.GetUserRolesAsync(identityUser1)).ReturnsAsync(roles);

            IQueryable<BuildingTeamMapping> queryableBuildingTeamMappings = buildingTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllBuildingTeamMappings()).Returns(queryableBuildingTeamMappings);
            IQueryable<JobDoneTeamMapping> queryableJobDoneTeamMappings = jobDoneTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllJobDoneTeamMappings()).Returns(queryableJobDoneTeamMappings);

            IQueryable<Team> queryableTeams = teams.BuildMock();
            mockTeamRepository.Setup(repo => repo.GetAllAttached()).Returns(queryableTeams);

            var result = await teamService.GetByIdAsync(team1.Id.ToString());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<TeamViewModel>(result);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldReturnTrue_WhenSuccessfullyDeleted()
        {
            var teamId = Guid.NewGuid().ToString();

            mockTeamRepository.Setup(repo => repo.SoftDeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await teamService.SoftDeleteAsync(teamId);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldReturnFalse_WhenDeleteFails()
        {
            var teamId = Guid.NewGuid().ToString();

            mockTeamRepository.Setup(repo => repo.SoftDeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await teamService.SoftDeleteAsync(teamId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllTeams_WhenNoDeletedTeams()
        {
            IQueryable<Team> queryableTeams = teams.BuildMock();

            mockTeamRepository.Setup(repo => repo.GetAllAttached()).Returns(queryableTeams);

            IQueryable<EmployeeTeamMapping> queryableEmployeeTeamMappings = employeeTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllEmployeeTeamMаppings()).Returns(queryableEmployeeTeamMappings);
            IQueryable<BuildingTeamMapping> queryableBuildingTeamMappings = buildingTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllBuildingTeamMappings()).Returns(queryableBuildingTeamMappings);
            IQueryable<JobDoneTeamMapping> queryableJobDoneTeamMappings = jobDoneTeamMappings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllJobDoneTeamMappings()).Returns(queryableJobDoneTeamMappings);

            var result = await teamService.GetAllAsync();

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task AddAsync_ShouldThrowException_WhenTeamWithSameNameExists()
        {
            var model = new TeamInputModel { Name = "Existing Team" };

            mockTeamRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync(new Team { Name = "Existing Team" });

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => teamService.AddAsync(model));
            Assert.That(ex.Message, Is.EqualTo("A team with this name already exists!"));
        }

        [Test]
        public async Task AddAsync_ShouldAddTeam_WhenValidModel()
        {
            var model = new TeamInputModel
            {
                Name = "New Team",
                SelectedBuildingId = building1.Id,
                SelectedEmployeeIds = new List<Guid>{employee1.Id}
            };

            mockTeamRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync((Team)null);

            mockHelpMethodsService.Setup(repo => repo.GetUserId()).Returns(identityUser1.Id);

            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(identityUser1.Id))
                .Returns(Guid.Parse(identityUser1.Id));

            IQueryable<Building> queryableBuildings = Buildings.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllBuildings()).Returns(queryableBuildings);

            IQueryable<Employee> queryableEmployees = employees.BuildMock();
            mockHelpMethodsService.Setup(repo => repo.GetAllEmployees()).Returns(queryableEmployees);

            var result = await teamService.AddAsync(model);

            mockTeamRepository.Verify(repo => repo.AddAsync(It.IsAny<Team>()), Times.Once);
            Assert.NotNull(result);
        }

    }
}