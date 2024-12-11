namespace ElProAppServices.Test
{
    using NUnit.Framework;
    using Moq;
    using MockQueryable;

    using ElProApp.Web.Models.JobDone;
    using ElProApp.Data.Models;
    using ElProApp.Data.Models.Mappings;

    [TestFixture]
    public class JobDoneServiceTests : ConstructorForMock
    {
        [Test]
        public async Task AddAsync_ShouldThrowException_WhenJobNotFound()
        {
            IQueryable<Job> queryableJobs = jobs.BuildMock();

            mockJobService.Setup(repo => repo.GetAllAttached())
                .Returns(queryableJobs);

            
            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => jobDoneService.AddAsync(jobDoneInputModel));
            Assert.AreEqual("Job not found.", exception.Message);
        }
        
        [Test]
        public async Task EditByIdAsync_ShouldThrowException_WhenJobDoneNotFound()
        {
            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(jobDone1.Id.ToString())).Returns(jobDone1.Id);
            
            List<JobDoneEditInputModel> emptyJobDones = new List<JobDoneEditInputModel>()
            {
                new JobDoneEditInputModel()
                {
                    Building = jobDone1.Building,
                    BuildingId = jobDone1.BuildingId,
                    DaysForJob = jobDone1.DaysForJob,
                    Id = jobDone1.Id,
                    Job = jobDone1.Job,
                    JobId = jobDone1.JobId,
                    Name = jobDone1.Name,
                    quantity = jobDone1.Quantity,
                    Team = new Team(){Name = "test1"},
                    TeamId = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77")
                },
                new JobDoneEditInputModel()
                {
                    Building = jobDone2.Building,
                    BuildingId = jobDone2.BuildingId,
                    DaysForJob = jobDone2.DaysForJob,
                    Id = jobDone2.Id,
                    Job = jobDone2.Job,
                    JobId = jobDone2.JobId,
                    Name = jobDone2.Name,
                    quantity = jobDone2.Quantity,
                    Team = new Team(){Name = "test2"},
                    TeamId = Guid.Parse("a27cfed2-abc4-4b6f-9447-3a77c43bf999")
                }
            };

            IQueryable<JobDone> queryableJobDOnes = jobDones.BuildMock();
            mockJobDoneRepository.Setup(repo => repo.GetAllAttached())
                .Returns(queryableJobDOnes);
            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(jobDone1.JobId.ToString()))
                .Returns(jobDone1.JobId);
            mockHelpMethodsService.Setup(repo => repo.GetTeamInforamtion(jobDone1.Id)).ReturnsAsync(team1);

            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => jobDoneService.EditByIdAsync(jobDone1.JobId.ToString()));
            Assert.AreEqual("JobDone is deleted or not found.", exception.Message);
        }

        [Test]
        public async Task EditByModelAsync_ShouldReturnTrue_WhenJobDoneUpdatedSuccessfully()
        {
            var updatedJobDone = jobDone1;
            mockJobDoneRepository.Setup(repo => repo.GetByIdAsync(jobDone1.Id))
                .ReturnsAsync(updatedJobDone);

            var result = await jobDoneService.EditByModelAsync(new JobDoneEditInputModel
            {
                Id = jobDone1.Id,
                Name = "Updated Description"
            });

            Assert.IsTrue(result);
            mockJobDoneRepository.Verify(repo => repo.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task EditByModelAsync_ShouldReturnFalse_WhenExceptionOccurs()
        {
            mockJobDoneRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).Throws(new Exception());

            var result = await jobDoneService.EditByModelAsync(new JobDoneEditInputModel
            {
                Id = Guid.NewGuid(),
                Name = "Faulty Update"
            });

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllJobDoneRecords()
        {
            List<JobDone> jobDones = new List<JobDone>()
            {
                jobDone1,
                jobDone2,
            };

            Team team = new Team()
            {
                Id = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77"),
                Name = "Първи екип",
                CreatedDate = DateTime.Now,
                IsDeleted = false

            };

            mockJobDoneRepository.Setup(repo => repo.GetAllAttached())
                .Returns(jobDones.AsQueryable());

            mockHelpMethodsService.Setup(repo => repo.GetTeamInforamtion(It.IsAny<Guid>())).ReturnsAsync(team);

            var result = await jobDoneService.GetAllAsync();

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetByIdAsync_ShouldThrowException_WhenJobDoneNotFound()
        {
            List<JobDone> jobDones = new List<JobDone>()
            {
                jobDone1,
                jobDone2,
            };
            IQueryable<JobDone> queryableJobDones = jobDones.BuildMock();

            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(team1.Id.ToString()))
                .Returns(team1.Id);
            mockJobDoneRepository.Setup(repo => repo.GetAllAttached())
                .Returns(queryableJobDones);

            var exception = Assert.ThrowsAsync<ArgumentException>(() => jobDoneService.GetByIdAsync(team1.Id.ToString()));
            Assert.AreEqual("Missing entity.", exception.Message);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnJobDone_WhenFound()
        {
            List<JobDone> jobDones = new List<JobDone>()
            {
                jobDone1,
                jobDone2,
            };
            IQueryable<JobDone> queryableJobDones = jobDones.BuildMock();

            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(jobDone1.Id.ToString()))
                .Returns(jobDone1.Id);
            mockJobDoneRepository.Setup(repo => repo.GetAllAttached())
                .Returns(queryableJobDones);

            mockJobDoneTeamMappingService.Setup(repo => repo.GetByJobDoneIdAsync(jobDone1.Id)).ReturnsAsync(jobDoneTeamMapping1);
            var result = await jobDoneService.GetByIdAsync(jobDone1.Id.ToString());

            Assert.IsNotNull(result);
            Assert.AreEqual(jobDone1.Id, result.Id);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldReturnTrue_WhenJobDoneDeletedSuccessfully()
        {
            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(jobDone1.Id.ToString()))
                .Returns(jobDone1.Id);

            mockJobDoneRepository.Setup(repo => repo.GetByIdAsync(jobDone1.Id))
                .ReturnsAsync(jobDone1);

            mockJobDoneRepository.Setup(repo => repo.SoftDeleteAsync(jobDone1.Id))
                .ReturnsAsync(true);

            var result = await jobDoneService.SoftDeleteAsync(jobDone1.Id.ToString());

            Assert.IsTrue(result);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldThrowException_WhenJobDoneNotFound()
        {
            mockJobDoneRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((JobDone)null);

            var result = await jobDoneService.SoftDeleteAsync(Guid.NewGuid().ToString());

            Assert.IsFalse(result);
        }
    }

}

