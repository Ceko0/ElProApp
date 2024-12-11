namespace ElProAppServices.Test
{
    using NUnit.Framework;

    using System.Linq;
    using System.Threading.Tasks;
    
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using ElProApp.Data;
    using ElProApp.Data.Models;
    using ElProApp.Services.Data;

    public class SearchServiceTests : ConstructorForMock
    {
        private ElProAppDbContext dbContext;
        private SearchService searchService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ElProAppDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDb")
                .Options;

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(new ElProAppDbContext(options));

            dbContext = new ElProAppDbContext(options);
            searchService = new SearchService(dbContext);
        }

        [Test]
        public async Task SearchAsync_ShouldReturnAllEntities_WhenQueryAndSearchInAreEmpty()
        {
            if(!dbContext.Set<Employee>().Contains(employee1)) await dbContext.Set<Employee>().AddAsync(employee1);
            if(!dbContext.Set<Employee>().Contains(employee2)) await dbContext.Set<Employee>().AddAsync(employee2);

            await dbContext.SaveChangesAsync();

            var result = await searchService.SearchAsync(null, null, 1, 10);

            Assert.AreEqual(employees.Count, result.TotalResults);
            Assert.AreEqual(employees.Count, result.Results.Count);
        }

        [Test]
        public async Task SearchAsync_ShouldReturnOnlyEmployee_WhenAreMoreThenOneTypeInContext()
        {
            if (!dbContext.Set<Employee>().Contains(employee1)) await dbContext.Set<Employee>().AddAsync(employee1);
            if (!dbContext.Set<Employee>().Contains(employee2)) await dbContext.Set<Employee>().AddAsync(employee2);
            if (!dbContext.Set<Job>().Contains(job1)) await dbContext.Set<Job>().AddAsync(job1);
            if (!dbContext.Set<Job>().Contains(job2)) await dbContext.Set<Job>().AddAsync(job2);

            await dbContext.SaveChangesAsync();

            var result = await searchService.SearchAsync(null, "Employee", 1, 10);

            Assert.AreEqual(employees.Count, result.TotalResults);
            Assert.AreEqual(employees.Count, result.Results.Count);
        }

        [Test]
        public async Task SearchAsync_ShouldFilterEntities_WhenQueryProvided()
        {
            if (!dbContext.Set<Employee>().Contains(employee1)) await dbContext.Set<Employee>().AddAsync(employee1);
            if (!dbContext.Set<Employee>().Contains(employee2)) await dbContext.Set<Employee>().AddAsync(employee2);

            await dbContext.SaveChangesAsync();

            var result = await searchService.SearchAsync(employee1.Name, "Employee", 1, 10);

            Assert.AreEqual(1, result.TotalResults);
            Assert.AreEqual(employee1.Name, (result.Results[0] as Employee)?.Name);
        }

        [Test]
        public async Task SearchAsync_ShouldReturnPaginatedResults()
        {
            if (!dbContext.Set<Employee>().Contains(employee1)) await dbContext.Set<Employee>().AddAsync(employee1);
            if (!dbContext.Set<Employee>().Contains(employee2)) await dbContext.Set<Employee>().AddAsync(employee2);

            await dbContext.SaveChangesAsync();

            var result = await searchService.SearchAsync(null, "Employee", 1, 2);

            Assert.AreEqual(2, result.Results.Count);
        }

        [Test]
        public async Task SearchAsync_ShouldReturnEmpty_WhenSearchInIsInvalid()
        {
            if (!dbContext.Set<Employee>().Contains(employee1)) await dbContext.Set<Employee>().AddAsync(employee1);
            if (!dbContext.Set<Employee>().Contains(employee2)) await dbContext.Set<Employee>().AddAsync(employee2);

            await dbContext.SaveChangesAsync();

            var result = await searchService.SearchAsync(null, "InvalidEntity", 1, 10);

            Assert.AreEqual(0, result.TotalResults);
            Assert.IsEmpty(result.Results); 
        }

        [Test]
        public async Task SearchAsync_ShouldReturnEmpty_WhenNoMatchFound()
        {
            if (!dbContext.Set<Employee>().Contains(employee1)) await dbContext.Set<Employee>().AddAsync(employee1);
            if (!dbContext.Set<Employee>().Contains(employee2)) await dbContext.Set<Employee>().AddAsync(employee2);

            await dbContext.SaveChangesAsync();

            var result = await searchService.SearchAsync("NonExistingEmployee", "Employee", 1, 10);

            Assert.AreEqual(0, result.TotalResults); 
            Assert.IsEmpty(result.Results);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext?.Dispose();
        }
    }
}