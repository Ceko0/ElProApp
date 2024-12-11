namespace ElProAppServices.Test
{
    using Moq;
    using MockQueryable;

    using System.Linq.Expressions;
    
    using ElProApp.Data.Models;
    using ElProApp.Services.Mapping;
    using ElProApp.Web.Models;
    using ElProApp.Web.Models.Employee;

    [TestFixture]
    public class EmployeeServiceTests : ConstructorForMock
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).Assembly);
        }

        [Test]
        public async Task AddAsync_ShouldThrowException_WhenEmployeeFirstNameIsEmpty()
        {
            var model = new EmployeeInputModel
            {
                Name = "",
                LastName = employee1.LastName,
                Wages = employee1.Wages
            };

            mockHelpMethodsService.Setup(repo => repo.GetUserId())
                .Returns(employee1.UserId);
            mockEmployeeRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Employee, bool>>>()))
                .ReturnsAsync(employee2);

            var exception = Assert.ThrowsAsync<ArgumentException>(() => employeeService.AddAsync(model));
            Assert.AreEqual("Employee first name must be provided.", exception.Message);
        }

        [Test]
        public async Task AddAsync_ShouldThrowException_WhenEmployeeAlreadyExists()
        {

            var model = new EmployeeInputModel
            {
                Name = employee1.Name,
                LastName = employee1.LastName,
                Wages = employee1.Wages
            };

            mockHelpMethodsService.Setup(repo => repo.GetUserId())
                .Returns(employee1.UserId);

            mockEmployeeRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Employee, bool>>>()))
                .ReturnsAsync(employee1);

            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => employeeService.AddAsync(model));
            Assert.AreEqual("Employee already exists for this user.", exception.Message);
        }

        [Test]
        public async Task AddAsync_ShouldReturnId_WhenEmployeeIsAddedSuccessfully()
        {
            var model = new EmployeeInputModel
            {
                Name = employee1.Name,
                LastName = employee1.LastName,
                Wages = employee1.Wages
            };

            mockEmployeeRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Employee, bool>>>()))
                .ReturnsAsync((Employee)null);

            var result = await employeeService.AddAsync(model);

            var isvalidGuid = Guid.TryParse(result, out Guid noNeed);
            Assert.IsNotNull(result);
            Assert.IsTrue(isvalidGuid);
        }

        [Test]
        public async Task EditByModelAsync_ShouldThrowException_WhenEmployeeNotFound()
        {
            var model = new EmployeeEditInputModel
            {
                Id = employee1.Id,
                Name = employee1.Name,
                LastName = employee1.LastName,
                Wages = employee1.Wages
            };

            mockEmployeeRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Employee)null);

            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => employeeService.EditByModelAsync(model));
            Assert.AreEqual("Employee is deleted or not found.", exception.Message);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldThrowException_WhenEmployeeIdIsInvalid()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(() => employeeService.SoftDeleteAsync(""));
            Assert.AreEqual("Invalid employee ID.", exception.Message);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldReturnTrue_WhenEmployeeIsSuccessfullyDeleted()
        {
            var id = employee1.Id;

            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(employee1.Id.ToString()))
                .Returns(employee1.Id);
            mockHelpMethodsService.Setup(repo => repo.GetCurrentUserAsync()).ReturnsAsync(identityUser1);

            mockEmployeeRepository.Setup(repo => repo.SoftDeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await employeeService.SoftDeleteAsync(employee1.Id.ToString());

            Assert.IsTrue(result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllEmployees_WhenEmployeesExist()
        {
            var employees = new List<Employee>
            {
                employee1,
                employee2
            };

            IQueryable<Employee> queryableEmployees = employees.BuildMock();
            mockEmployeeRepository.Setup(repo => repo.GetAllAttached())
                .Returns(queryableEmployees);

            var result = await employeeService.GetAllAsync();

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetByIdAsync_ShouldThrowException_WhenEmployeeNotFound()
        {
            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => employeeService.GetByIdAsync(Guid.NewGuid().ToString()));
            Assert.AreEqual("Employee is deleted or not found.", exception.Message);
        }

        [Test]
        public async Task GetByIdAsync_ShouldThrowException_WhenEmployeeIdIsEmpty()
        {
            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => employeeService.GetByIdAsync(""));
            Assert.AreEqual("Invalid employee ID.", exception.Message);
        }

        [Test]
        public async Task EditByIdAsync_ShouldThrowException_WhenIdIsInvalid()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(() => employeeService.EditByIdAsync(""));
            Assert.AreEqual("Invalid employee ID.", exception.Message);
        }

        [Test]
        public async Task EditByIdAsync_ShouldThrowException_WhenEmployeeNotFound()
        {
            var id = Guid.NewGuid().ToString();
            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(id)).Returns(Guid.Parse(id));
            mockEmployeeRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Employee)null);

            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => employeeService.EditByIdAsync(id));
            Assert.AreEqual("Employee is deleted or not found.", exception.Message);
        }

        [Test]
        public async Task EditByIdAsync_ShouldReturnEditModel_WhenEmployeeExists()
        {
            mockHelpMethodsService.Setup(repo => repo.ConvertAndTestIdToGuid(employee1.Id.ToString())).Returns(employee1.Id);
            mockEmployeeRepository.Setup(repo => repo.GetByIdAsync(employee1.Id)).ReturnsAsync(employee1);
            mockHelpMethodsService.Setup(repo => repo.GetUserAsync(employee1.UserId)).ReturnsAsync(identityUser1);

            var result = await employeeService.EditByIdAsync(employee1.Id.ToString());

            Assert.IsNotNull(result);
            Assert.AreEqual(employee1.Name, result.Name);
            Assert.AreEqual(employee1.LastName, result.LastName);
        }

        [Test]
        public void GetByUserId_ShouldThrowException_WhenUserIdIsInvalid()
        {
            var exception = Assert.Throws<ArgumentException>(() => employeeService.GetByUserId(""));
            Assert.AreEqual("Invalid user ID.", exception.Message);
        }

        [Test]
        public void GetByUserId_ShouldThrowException_WhenEmployeeNotFound()
        {
            mockEmployeeRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Employee>().BuildMock());

            var exception = Assert.Throws<InvalidOperationException>(() => employeeService.GetByUserId(identityUser1.Id));
            Assert.AreEqual("Employee not found for the specified user.", exception.Message);
        }

        [Test]
        public void GetByUserId_ShouldReturnEmployee_WhenEmployeeExists()
        {
            var employees = new List<Employee> { employee1 }.AsQueryable();
            mockEmployeeRepository.Setup(repo => repo.GetAllAttached()).Returns(employees.BuildMock());

            var result = employeeService.GetByUserId(employee1.UserId);

            Assert.IsNotNull(result);
            Assert.AreEqual(employee1.Id, result.Id);
            Assert.AreEqual(employee1.Name, result.Name);
        }

        [Test]
        public void GetAllAttached_ShouldReturnAllAttachedEmployees()
        {
            var employees = new List<Employee> { employee1, employee2 }.AsQueryable();
            mockEmployeeRepository.Setup(repo => repo.GetAllAttached()).Returns(employees.BuildMock());

            var result = employeeService.GetAllAttached();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

    }
}
