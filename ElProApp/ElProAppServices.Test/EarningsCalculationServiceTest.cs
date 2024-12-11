namespace ElProAppServices.Test
{
    using NUnit.Framework;
    using Moq;
    using MockQueryable;

    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using ElProApp.Web.Models.JobDone;
    using ElProApp.Data.Models;
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Web.Models.Job;

    [TestFixture]
    public class EarningsCalculationServiceTests : ConstructorForMock
    {
        [Test]
        public async Task CalculateMoneyAsync_ShouldCalculateCorrectly_WhenValidData()
        {
            IQueryable<EmployeeTeamMapping> queryableEmployeeTeamMappings = employeeTeamMappings.BuildMock();
            mockEmployeeTeamMappingService.Setup(service => service.GetAllAttached())
                .Returns(queryableEmployeeTeamMappings);

            var jobViewModel = new JobViewModel()
            {
                Id = job1.Id,
                Name = job1.Name,
                Price = job1.Price
            };

            mockJobService.Setup(service => service.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(jobViewModel);

            IQueryable<Employee> queryableEmployees = employees.BuildMock();
            mockEmployeeService.Setup(service => service.GetAllAttached())
                .Returns(queryableEmployees);

            var jobDoneInputModel = new JobDoneInputModel()
            {
                BuildingId = jobDone1.BuildingId,
                buildings = new List<Building>() { building1, building2 },
                DaysForJob = jobDone1.DaysForJob,
                Id = jobDone1.Id,
                JobId = jobDone1.JobId,
                jobs = new List<Job>() { job1, job2 },
                Name = jobDone1.Name,
                Quantity = jobDone1.Quantity,
                TeamId = team1.Id,
                teams = new List<Team>() { team1, team2 }
            };

            mockEmployeeService.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);
            var result = await earningsCalculationService.CalculateMoneyAsync(jobDoneInputModel);

            Assert.IsTrue(result);
            Assert.AreEqual(4410, employee1.MoneyToTake);
            Assert.AreEqual(50, employee2.MoneyToTake);
        }
    }
}


