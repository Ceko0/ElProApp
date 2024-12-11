namespace ElProAppServices.Test
{
    using Moq;
    using NUnit.Framework;
    using MockQueryable;

    using System.Linq;
    using System.Threading.Tasks;

    using ElProApp.Data.Models.Mappings;

    [TestFixture]
    public class BuildingTeamMappingServiceTests : ConstructorForMock
    {
        [Test]
        public async Task GetAllAttachedAsync_ShouldReturnAllMappings()
        {
            IQueryable<BuildingTeamMapping> queryableMappings = buildingTeamMappings.BuildMock();
            mockBuildingTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableMappings);

            var result = await buildingTeamMappingService.GetAllAttachedAsync();

            Assert.AreEqual(buildingTeamMappings.Count, result.Count);
            CollectionAssert.AreEquivalent(buildingTeamMappings, result);
        }

        [Test]
        public async Task GetByTeamIdAsync_ShouldReturnMappingsForSpecificTeam()
        {
            var teamId = buildingTeamMapping1.TeamId;
            IQueryable<BuildingTeamMapping> queryableMappings = buildingTeamMappings.BuildMock();
            mockBuildingTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableMappings);

            var result = await buildingTeamMappingService.GetByTeamIdAsync(teamId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(buildingTeamMapping1, result.First());
        }

        [Test]
        public async Task GetByBuildingIdAsync_ShouldReturnMappingsForSpecificBuilding()
        {
            var buildingId = buildingTeamMapping1.BuildingId;
            IQueryable<BuildingTeamMapping> queryableMappings = buildingTeamMappings.BuildMock();
            mockBuildingTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableMappings);

            var result = await buildingTeamMappingService.GetByBuildingIdAsync(buildingId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(buildingTeamMapping1, result.First());
        }

        [Test]
        public async Task AddAsync_ShouldAddNewMapping()
        {
            var result = await buildingTeamMappingService.AddAsync(buildingTeamMapping1.BuildingId, buildingTeamMapping1.TeamId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BuildingTeamMapping>(result);
        }

        [Test]
        public async Task RemoveAsync_ShouldRemoveMapping()
        {
            var mappingToRemove = buildingTeamMapping1;

            IQueryable<BuildingTeamMapping> queryableMappings = buildingTeamMappings.BuildMock();
            mockBuildingTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableMappings);

            mockBuildingTeamMappingRepository
                .Setup(r => r.DeleteByCompositeKeyAsync(buildingTeamMapping1.BuildingId, buildingTeamMapping1.TeamId))
                .ReturnsAsync(true);

            bool result = await buildingTeamMappingService.RemoveAsync(mappingToRemove);

            Assert.IsTrue(result);
        }

        [Test]
        public void Any_ShouldReturnTrueIfMappingExists()
        {
            var buildingId = buildingTeamMapping1.BuildingId;
            var teamId = buildingTeamMapping1.TeamId;

            mockBuildingTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(buildingTeamMappings.AsQueryable());

            var result = buildingTeamMappingService.Any(buildingId, teamId);

            Assert.IsTrue(result);
        }

        [Test]
        public void Any_ShouldReturnFalseIfMappingDoesNotExist()
        {
            var buildingId = Guid.NewGuid();
            var teamId = Guid.NewGuid();

            mockBuildingTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(buildingTeamMappings.AsQueryable());

            var result = buildingTeamMappingService.Any(buildingId, teamId);

            Assert.IsFalse(result);
        }
    }
}