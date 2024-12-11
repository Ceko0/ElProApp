namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using Interfaces;
    using Web.Models.JobDone;

    public class EarningsCalculationService(IServiceProvider serviceProvider) : IEarningsCalculationService
    {
        public async Task<bool> CalculateMoneyAsync(JobDoneInputModel model)
        {
            var employeeTeamMappingService = serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();
            var employeesId = await employeeTeamMappingService
                .GetAllAttached()
                .Where(x => x.TeamId == model.TeamId)
                .Select(x => x.EmployeeId)
                .ToListAsync();

            var jobService = serviceProvider.GetRequiredService<IJobService>();
            var job = await jobService.GetByIdAsync(model.JobId.ToString());

            var totalMoney = job.Price * model.Quantity;

            var moneyForEmployee = totalMoney / employeesId.Count();

            var employeeService = serviceProvider.GetRequiredService<IEmployeeService>();

            var employees = employeeService.GetAllAttached().Where(x => employeesId.Contains(x.Id));

            foreach (var employee in employees)
            {
                var takenMoneyFromEmployee = employee.Wages * model.DaysForJob;
                var moneyToTake = moneyForEmployee - takenMoneyFromEmployee;
                employee.MoneyToTake += moneyToTake;                
            }

            return  await employeeService.SaveChangesAsync();
        }
    }
}
