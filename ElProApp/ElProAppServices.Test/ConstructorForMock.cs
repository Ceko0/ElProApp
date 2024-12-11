namespace ElProAppServices.Test
{
    using NUnit.Framework;
    using Moq;

    using Microsoft.AspNetCore.Identity;

    using ElProApp.Data.Repository.Interfaces;
    using ElProApp.Services.Data.Interfaces;
    using ElProApp.Services.Data;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models.JobDone;
    using ElProApp.Data.Models;
    using ElProApp.Data.Models.Mappings;

    public class ConstructorForMock
    {
        protected JobDoneService jobDoneService;
        protected TeamService teamService;
        protected BuildingService buildingService;
        protected EmployeeService employeeService;
        protected JobService jobService;
        protected EmployeeTeamMappingService employeeTeamMappingService;
        protected BuildingTeamMappingService buildingTeamMappingService;
        protected JobDoneTeamMappingService jobDoneTeamMappingService;

        protected EarningsCalculationService earningsCalculationService;

        protected Mock<IServiceProvider> mockServiceProvider;
        protected Mock<IHelpMethodsService> mockHelpMethodsService;

        protected Mock<IRepository<JobDone, Guid>> mockJobDoneRepository;
        protected Mock<IRepository<Team, Guid>> mockTeamRepository;
        protected Mock<IRepository<Job, Guid>> mockJobRepository;
        protected Mock<IRepository<Employee, Guid>> mockEmployeeRepository;
        protected Mock<IRepository<Building, Guid>> mockBuildingRepository;
        protected Mock<IRepository<BuildingTeamMapping, Guid>> mockBuildingTeamMappingRepository;
        protected Mock<IRepository<JobDoneTeamMapping, Guid>> mockJobDoneTeamMappingRepository;
        protected Mock<IRepository<EmployeeTeamMapping, Guid>> mockEmployeeTeamMappingRepository;

        protected Mock<IJobService> mockJobService;
        protected Mock<IBuildingService> mockBuildingService;
        protected Mock<ITeamService> mockTeamService;
        protected Mock<IJobDoneTeamMappingService> mockJobDoneTeamMappingService;
        protected Mock<IBuildingTeamMappingService> mockBuildingTeamMappingService;
        protected Mock<IEarningsCalculationService> mockEarningsCalculationService;
        protected Mock<IEmployeeService> mockEmployeeService;
        protected Mock<IEmployeeTeamMappingService> mockEmployeeTeamMappingService;

        protected Employee employee1;
        protected Employee employee2;
        protected List<Employee> employees;

        protected IdentityUser identityUser1;
        protected IdentityUser identityUser2;
        protected List<IdentityUser> IdentityUsers;

        protected JobDone jobDone1;
        protected JobDone jobDone2;
        protected JobDoneInputModel jobDoneInputModel;
        protected List<JobDone> jobDones;

        protected Job job1;
        protected Job job2;
        protected List<Job> jobs;

        protected JobDoneTeamMapping jobDoneTeamMapping1;
        protected JobDoneTeamMapping jobDoneTeamMapping2;
        protected List<JobDoneTeamMapping> jobDoneTeamMappings;

        protected Team team1;
        protected Team team2;
        protected List<Team> teams;

        protected Building building1;
        protected Building building2;
        protected List<Building> Buildings;

        protected BuildingTeamMapping buildingTeamMapping1;
        protected BuildingTeamMapping buildingTeamMapping2;
        protected List<BuildingTeamMapping> buildingTeamMappings;

        protected EmployeeTeamMapping employeeTeamMapping1;
        protected EmployeeTeamMapping employeeTeamMapping2;
        protected List<EmployeeTeamMapping> employeeTeamMappings;

        [SetUp]
        public void SetUp()
        {
            mockServiceProvider = new Mock<IServiceProvider>();
            mockHelpMethodsService = new Mock<IHelpMethodsService>();

            mockJobDoneRepository = new Mock<IRepository<JobDone, Guid>>();
            mockTeamRepository = new Mock<IRepository<Team, Guid>>();
            mockJobRepository = new Mock<IRepository<Job, Guid>>();
            mockEmployeeRepository = new Mock<IRepository<Employee, Guid>>();
            mockBuildingRepository = new Mock<IRepository<Building, Guid>>();
            mockBuildingTeamMappingRepository = new Mock<IRepository<BuildingTeamMapping, Guid>>();
            mockJobDoneTeamMappingRepository = new Mock<IRepository<JobDoneTeamMapping, Guid>>();
            mockEmployeeTeamMappingRepository = new Mock<IRepository<EmployeeTeamMapping, Guid>>();

            mockJobService = new Mock<IJobService>();
            mockBuildingService = new Mock<IBuildingService>();
            mockTeamService = new Mock<ITeamService>();
            mockJobDoneTeamMappingService = new Mock<IJobDoneTeamMappingService>();
            mockBuildingTeamMappingService = new Mock<IBuildingTeamMappingService>();
            mockEarningsCalculationService = new Mock<IEarningsCalculationService>();
            mockEmployeeService = new Mock<IEmployeeService>();
            mockEmployeeTeamMappingService = new Mock<IEmployeeTeamMappingService>();

            mockServiceProvider.Setup(sp => sp.GetService(typeof(IJobService))).Returns(mockJobService.Object);
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IBuildingService))).Returns(mockBuildingService.Object);
            mockServiceProvider.Setup(sp => sp.GetService(typeof(ITeamService))).Returns(mockTeamService.Object);
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IEmployeeService))).Returns(mockEmployeeService.Object);
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IJobDoneTeamMappingService))).Returns(mockJobDoneTeamMappingService.Object);
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IBuildingTeamMappingService))).Returns(mockBuildingTeamMappingService.Object);
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IEmployeeTeamMappingService))).Returns(mockEmployeeTeamMappingService.Object);

            jobDoneService = new JobDoneService(mockJobDoneRepository.Object,
                mockServiceProvider.Object,
                mockHelpMethodsService.Object);
            teamService = new TeamService(mockTeamRepository.Object,
                mockServiceProvider.Object,
                mockHelpMethodsService.Object);
            buildingService = new BuildingService(mockBuildingRepository.Object,
                mockBuildingTeamMappingService.Object,
                mockHelpMethodsService.Object);
            employeeService = new EmployeeService(mockEmployeeRepository.Object,
                mockEmployeeTeamMappingService.Object,
                mockHelpMethodsService.Object);
            jobService = new JobService(mockJobRepository.Object, 
                mockHelpMethodsService.Object);
            employeeTeamMappingService = new EmployeeTeamMappingService(mockEmployeeTeamMappingRepository.Object);
            buildingTeamMappingService = new BuildingTeamMappingService(mockBuildingTeamMappingRepository.Object);
            jobDoneTeamMappingService = new JobDoneTeamMappingService(mockJobDoneTeamMappingRepository.Object);
            earningsCalculationService = new EarningsCalculationService(mockServiceProvider.Object);

            employee1 = new()
            {
                Id = Guid.Parse("b3219f94-7541-4f5c-8238-35fdc97e9b43"),
                Name = "Цветомир",
                LastName = "Иванов",
                Wages = 120,
                MoneyToTake = 30,
                IsDeleted = false,
                UserId = "3f11c6e3-ec7b-4c56-b2f4-ecf77c3d4b88"
            };

            employee2 = new()
            {
                Id = Guid.Parse("c53db596-5871-4048-9b39-72a7dcd8143c"),
                Name = "Петър",
                LastName = "Петров",
                Wages = 100,
                MoneyToTake = 50,
                IsDeleted = false,
                UserId = "cc8c5c1f -b0c9-4e3f-8eb3-e70c5e74d63b"
            };
            employees = new() { employee1, employee2 };

            identityUser1 = new()
            {
                Id = "3f11c6e3-ec7b-4c56-b2f4-ecf77c3d4b88",
                UserName = "Cvetomir",
                Email = "Cvetomir@example.com",
                PasswordHash = "Cvetomir",
                PhoneNumber = "0888888881"
            };

            identityUser2 = new()
            {
                Id = "cc8c5c1f-b0c9-4e3f-8eb3-e70c5e74d63b",
                UserName = "Petyr",
                Email = "Petyr@example.com",
                PasswordHash = "Petyr@",
                PhoneNumber = "0987654321"
            };
            IdentityUsers = new() { identityUser1, identityUser2 };

            jobDone1 = new JobDone
            {
                Id = Guid.Parse("f24ae2d4-75a8-4aef-b6f7-b1a5c4a12d89"),
                Name = "Полагане на кабел в Блок 178",
                IsDeleted = false,
                Building = new Building() { Name = "Блок 178" },
                BuildingId = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                CreatedDate = DateTime.Now,
                DaysForJob = 1,
                Quantity = 300,
                Job = new Job() { Name = "Полагане на кабел" },
                JobId = Guid.Parse("9f57e3a4-102a-4d3d-b19b-7e4f6e8a2b4c")

            };

            jobDone2 = new JobDone
            {
                Id = Guid.Parse("e5d8c7d5-92f4-4f7d-9d6c-a9e8d1f8d7c8"),
                Name = "Монтаж на осветително тяло в Блок 356",
                IsDeleted = false,
                Building = new Building() { Name = "Блок 356" },
                BuildingId = Guid.Parse("e50f6dd1-2bd8-409f-88a3-e1d11c12cd6d"),
                CreatedDate = DateTime.Now,
                DaysForJob = 2,
                Quantity = 150,
                Job = new Job() { Name = "Монтаж на осветително тяло" },
                JobId = Guid.Parse("fa6cd2c1-0f5e-4e4f-a3be-0a3df39b2e76")
            };
            jobDones = new() { jobDone1, jobDone2 };

            job1 = new Job
            {
                Id = Guid.Parse("9f57e3a4-102a-4d3d-b19b-7e4f6e8a2b4c"),
                CreatedDate = DateTime.Now,
                Name = "Job1",
                Price = 15,
                IsDeleted = false
            };

            job2 = new Job
            {
                Id = Guid.Parse("fa6cd2c1-0f5e-4e4f-a3be-0a3df39b2e76"),
                CreatedDate = DateTime.Now,
                Name = "Job2",
                Price = 20,
                IsDeleted = false
            };

            jobs = new() { job1, job2 };

            jobDoneInputModel = new JobDoneInputModel
            {
                Id = Guid.Parse("e5d8c7d5-92f4-4f7d-9d6c-a9e8d1f8d7c8"),
                Name = "Монтаж на осветително тяло в Блок 356",
                BuildingId = Guid.Parse("e50f6dd1-2bd8-409f-88a3-e1d11c12cd6d"),
                DaysForJob = 2,
                Quantity = 150,
                JobId = Guid.Parse("fa6cd2c1-0f5e-4e4f-a3be-0a3df39b2e36")
            };

            team1 = new Team()
            {
                Id = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77"),
                Name = "Първи екип",
                IsDeleted = false,
                CreatedDate = DateTime.Now
            };

            team2 = new Team()
            {
                Id = Guid.Parse("a27cfed2-abc4-4b6f-9447-3a77c43bf999"),
                Name = "Екип катеричка",
                IsDeleted = false,
                CreatedDate = DateTime.Now
            };
            teams = new() { team1, team2 };

            building1 = new Building()
            {
                CreatedDate = DateTime.Now,
                Id = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                Name = "Блок 178",
                Location = "София жк Банишоара ",
                IsDeleted = false
            };

            building2 = new Building()
            {
                CreatedDate = DateTime.Now,
                Id = Guid.Parse("e50f6dd1-2bd8-409f-88a3-e1d11c12cd6d"),
                Name = "Блок 356",
                Location = "София жк Надежда 1",
                IsDeleted = false
            };
            Buildings = new() { building1, building2 };

            buildingTeamMapping1 = new BuildingTeamMapping()
            {
                Id = Guid.Parse("2c65c863-0331-45c8-0671-08dd12317b1e"),
                BuildingId = building1.Id,
                Building = building1,
                TeamId = team1.Id,
                Team = team1,
                CreatedDate = DateTime.Now
            };

            buildingTeamMapping2 = new BuildingTeamMapping()
            {
                Id = Guid.Parse("0f20af7f-60b4-4db6-78ec-08dd17b423a8"),
                BuildingId = building2.Id,
                Building = building2,
                TeamId = team2.Id,
                Team = team2,
                CreatedDate = DateTime.Now
            };
            buildingTeamMappings = new() { buildingTeamMapping1, buildingTeamMapping2 };

            employeeTeamMapping1 = new()
            {
                CreatedDate = DateTime.Now,
                Id = Guid.Parse("50af47c7-7f05-4d6c-b7ba-8dbaa2760e0e"),
                EmployeeId = employee1.Id,
                Employee = employee1,
                TeamId = team1.Id,
                Team = team1
            };

            employeeTeamMapping2 = new()
            {
                CreatedDate = DateTime.Now,
                Id = Guid.Parse("2be5ce6c-dbbb-4a0a-a3c5-1608e38d6d4f"),
                EmployeeId = employee2.Id,
                Employee = employee2,
                TeamId = team2.Id,
                Team = team2
            };
            employeeTeamMappings = new() { employeeTeamMapping1, employeeTeamMapping2 };

            jobDoneTeamMapping1 = new JobDoneTeamMapping()
            {
                CreatedDate = DateTime.Now,
                Id = Guid.Parse("d12f12a0-c08e-443a-92d3-c95203f01466"),
                JobDoneId = jobDone1.Id,
                JobDone = jobDone1,
                TeamId = team1.Id,
                Team = team1
            };

            jobDoneTeamMapping2 = new JobDoneTeamMapping()
            {
                CreatedDate = DateTime.Now,
                Id = Guid.Parse("71048910-6815-4848-9b5d-ee2eeb361f38"),
                JobDoneId = jobDone2.Id,
                JobDone = jobDone2,
                TeamId = team2.Id,
                Team = team2
            };
            jobDoneTeamMappings = new() { jobDoneTeamMapping1, jobDoneTeamMapping2 };

            AutoMapperConfig.RegisterMappings(typeof(JobDoneViewModel).Assembly);
        }
    }
}
