namespace ElProAppServices.Test
{
    using Moq;
    using NUnit.Framework;
    using MockQueryable;

    using System.Linq;
    using System.Threading.Tasks;

    using ElProApp.Data.Models.Mappings;

    [TestFixture]
    public class JobDoneTeamMappingServiceTests : ConstructorForMock
    {
        [Test]
        public async Task GetAllAsync_ShouldReturnAllMappings()
        {
            IQueryable<JobDoneTeamMapping> queryableMappings = jobDoneTeamMappings.BuildMock();
            mockJobDoneTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableMappings);

            var result = await jobDoneTeamMappingService.GetAllAsync();

            Assert.AreEqual(jobDoneTeamMappings.Count, result.Count);
            CollectionAssert.AreEquivalent(jobDoneTeamMappings, result);
        }

        [Test]
        public async Task GetByTeamIdAsync_ShouldReturnMappingsForSpecificTeam()
        {
            var teamId = jobDoneTeamMapping1.TeamId;
            IQueryable<JobDoneTeamMapping> queryableMappings = jobDoneTeamMappings.BuildMock();
            mockJobDoneTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableMappings);

            var result = await jobDoneTeamMappingService.GetByTeamIdAsync(teamId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(jobDoneTeamMapping1, result.First());
        }

        [Test]
        public async Task GetByJobDoneIdAsync_ShouldReturnMappingsForSpecificJobDone()
        {
            var jobDoneId = jobDoneTeamMapping1.JobDoneId;
            IQueryable<JobDoneTeamMapping> queryableMappings = jobDoneTeamMappings.BuildMock();
            mockJobDoneTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableMappings);

            var result = await jobDoneTeamMappingService.GetByJobDoneIdAsync(jobDoneId);

            Assert.AreEqual(jobDoneTeamMapping1, result);
        }

        [Test]
        public async Task AddAsync_ShouldAddNewMapping()
        {
            var result = await jobDoneTeamMappingService.AddAsync(jobDoneTeamMapping1.JobDoneId, jobDoneTeamMapping1.TeamId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JobDoneTeamMapping>(result);
        }

        [Test]
        public async Task RemoveAsync_ShouldRemoveMapping()
        {
            var mappingToRemove = jobDoneTeamMapping1;

            IQueryable<JobDoneTeamMapping> queryableMappings = jobDoneTeamMappings.BuildMock();
            mockJobDoneTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(queryableMappings);

            mockJobDoneTeamMappingRepository
                .Setup(r => r.DeleteByCompositeKeyAsync(jobDoneTeamMapping1.JobDoneId, jobDoneTeamMapping1.TeamId))
                .ReturnsAsync(true);

            bool result = await jobDoneTeamMappingService.RemoveAsync(mappingToRemove);

            Assert.IsTrue(result);
        }

        [Test]
        public void Any_ShouldReturnTrueIfMappingExists()
        {
            var jobDoneId = jobDoneTeamMapping1.JobDoneId;
            var teamId = jobDoneTeamMapping1.TeamId;

            mockJobDoneTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(jobDoneTeamMappings.AsQueryable());

            var result = jobDoneTeamMappingService.Any(jobDoneId, teamId);

            Assert.IsTrue(result);
        }

        [Test]
        public void Any_ShouldReturnFalseIfMappingDoesNotExist()
        {
            var jobDoneId = Guid.NewGuid();
            var teamId = Guid.NewGuid();

            mockJobDoneTeamMappingRepository
                .Setup(r => r.GetAllAttached())
                .Returns(jobDoneTeamMappings.AsQueryable());

            var result = jobDoneTeamMappingService.Any(jobDoneId, teamId);

            Assert.IsFalse(result);
        }
    }
}