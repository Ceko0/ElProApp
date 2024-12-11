using ElProApp.Services.Data.Interfaces;

namespace ElProAppServices.Test
{
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    using ElProApp.Services.Data;
    using ElProApp.Data.Models;
    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Web.Models.Job;

    [TestFixture]
    public class JobServiceTests : ConstructorForMock
    {

        [Test]
        public async Task AddAsync_ShouldAddJobAndReturnJobId()
        {
            var jobInputModel = new JobInputModel
            {
            };

            var jobEntity = new Job
            {
                Id = Guid.NewGuid()
            };

            mockJobRepository.Setup(repo => repo.AddAsync(It.IsAny<Job>())).Returns(Task.CompletedTask);

            var result = await jobService.AddAsync(jobInputModel);

            Assert.NotNull(result);
            Assert.IsInstanceOf<string>(result);
            mockJobRepository.Verify(repo => repo.AddAsync(It.IsAny<Job>()), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnJob_WhenJobExists()
        {
            var jobId = Guid.NewGuid().ToString();
            var jobEntity = new Job
            {
                Id = Guid.Parse(jobId),
                IsDeleted = false
            };

            mockJobRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(jobEntity);

            var result = await jobService.GetByIdAsync(jobId);

            Assert.NotNull(result);
            Assert.IsInstanceOf<JobViewModel>(result);
            Assert.AreEqual(jobId, result.Id.ToString());
        }

        [Test]
        public async Task GetByIdAsync_ShouldThrowArgumentException_WhenJobIsDeleted()
        {
            var jobId = Guid.NewGuid().ToString();
            var jobEntity = new Job
            {
                Id = Guid.Parse(jobId),
                IsDeleted = true
            };

            mockJobRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(jobEntity);

            Assert.ThrowsAsync<ArgumentException>(async () => await jobService.GetByIdAsync(jobId));
        }

        [Test]
        public async Task EditByIdAsync_ShouldReturnJobEditInputModel_WhenJobExists()
        {
            var jobId = Guid.NewGuid().ToString();
            var jobEntity = new Job
            {
                Id = Guid.Parse(jobId),
                IsDeleted = false
            };

            mockJobRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(jobEntity);

            var result = await jobService.EditByIdAsync(jobId);

            Assert.NotNull(result);
            Assert.IsInstanceOf<JobEditInputModel>(result);
            Assert.AreEqual(jobId, result.Id.ToString());
        }

        [Test]
        public async Task EditByIdAsync_ShouldThrowInvalidOperationException_WhenJobIsDeleted()
        {
            var jobId = Guid.NewGuid().ToString();
            var jobEntity = new Job
            {
                Id = Guid.Parse(jobId),
                IsDeleted = true
            };

            mockJobRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(jobEntity);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await jobService.EditByIdAsync(jobId));
        }

        [Test]
        public async Task EditByModelAsync_ShouldReturnTrue_WhenUpdateIsSuccessful()
        {
            var jobId = Guid.NewGuid().ToString();
            var jobEditModel = new JobEditInputModel
            {
                Id = Guid.Parse(jobId),
                // Добавете необходимите полета за теста
            };

            var jobEntity = new Job
            {
                Id = Guid.Parse(jobId),
                IsDeleted = false
            };

            mockJobRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(jobEntity);

            var result = await jobService.EditByModelAsync(jobEditModel);

            Assert.IsTrue(result);
            mockJobRepository.Verify(repo => repo.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task EditByModelAsync_ShouldReturnFalse_WhenJobNotFound()
        {
            var jobEditModel = new JobEditInputModel
            {
                Id = Guid.NewGuid()
            };

            mockJobRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Job)null);

            var result = await jobService.EditByModelAsync(jobEditModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldReturnTrue_WhenSoftDeleteIsSuccessful()
        {
            var jobId = Guid.NewGuid().ToString();
            var jobEntity = new Job
            {
                Id = Guid.Parse(jobId),
                IsDeleted = false
            };

            mockJobRepository.Setup(repo => repo.SoftDeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await jobService.SoftDeleteAsync(jobId);

            Assert.IsTrue(result);
            mockJobRepository.Verify(repo => repo.SoftDeleteAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldReturnFalse_WhenSoftDeleteFails()
        {
            var jobId = Guid.NewGuid().ToString();

            mockJobRepository.Setup(repo => repo.SoftDeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await jobService.SoftDeleteAsync(jobId);

            Assert.IsFalse(result);
        }
    }
}