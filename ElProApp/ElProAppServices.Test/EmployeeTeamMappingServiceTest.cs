namespace ElProAppServices.Test
{
    using Moq;
    using NUnit.Framework;
    using MockQueryable;

    using System.Linq;
    using System.Threading.Tasks;

    using ElProApp.Data.Models.Mappings;

    [TestFixture]
    public class EmployeeTeamMappingServiceTests : ConstructorForMock
    {
        [Test]
        public async Task GetAllAsync_ShouldReturnAllMappings()
        {
            IQueryable<EmployeeTeamMapping> queryableEmployeeTeamMappings = employeeTeamMappings.BuildMock();
            mockEmployeeTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableEmployeeTeamMappings);

            var result = await employeeTeamMappingService.GetAllAsync();

            Assert.AreEqual(employeeTeamMappings.Count, result.Count);
            CollectionAssert.AreEquivalent(employeeTeamMappings, result);
        }

        [Test]
        public void GetAllByEmployeeId_ShouldReturnMappingsForSpecificEmployee()
        {
            var employeeId = employeeTeamMapping1.EmployeeId.ToString();
            mockEmployeeTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(employeeTeamMappings.AsQueryable());

            var result = employeeTeamMappingService.GetAllByEmployeeId(employeeId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(employeeTeamMapping1, result.First());
        }

        [Test]
        public async Task GetAllByTeamId_ShouldReturnMappingsForSpecificTeam()
        {
            var teamId = employeeTeamMapping1.TeamId;
            IQueryable<EmployeeTeamMapping> queryableEmployeeTeamMappings = employeeTeamMappings.BuildMock();
            mockEmployeeTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableEmployeeTeamMappings);

            var result = await employeeTeamMappingService.GetByTeamIdAsync(teamId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(employeeTeamMapping1, result.First());
        }

        [Test]
        public async Task AddMappingAsync_ShouldAddNewMapping()
        {
            var result = await employeeTeamMappingService.AddAsync(employeeTeamMapping1.EmployeeId, employeeTeamMapping1.TeamId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<EmployeeTeamMapping>(result);
        }


        [Test]
        public async Task RemoveMappingAsync_ShouldRemoveMapping()
        {
            var mappingToRemove = employeeTeamMapping1;

            IQueryable<EmployeeTeamMapping> queryableEmployeeTeamMappings = employeeTeamMappings.BuildMock();
            mockEmployeeTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableEmployeeTeamMappings);

            mockEmployeeTeamMappingRepository
                .Setup(r => r.DeleteByCompositeKeyAsync(employeeTeamMapping1.EmployeeId, employeeTeamMapping1.TeamId))
                .ReturnsAsync(true);

            bool result = await employeeTeamMappingService.RemoveAsync(mappingToRemove);

            Assert.IsTrue(result);
        }
    }
}