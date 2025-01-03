namespace ElProApp.Services.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using Interfaces;
    using Web.Models.JobDone;
    using static Common.EntityValidationConstants.CalculationAction;

    public class EarningsCalculationService(IServiceProvider serviceProvider) : IEarningsCalculationService
    {
        public async Task<bool> CalculateMoneyAsync(Guid teamId , Dictionary<Guid,decimal> jobs, Guid jobDoneId , int daysForJob,string action)
        {
            if (string.IsNullOrEmpty(action)) throw new AggregateException("action can`t be null or empty");

            bool isAdding = action == Add;
            bool isRemoving = action == Remove;

            var employeeTeamMappingService = serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();
            var jobService = serviceProvider.GetRequiredService<IJobService>();
            var jobDoneJobService = serviceProvider.GetRequiredService<IJobDoneJobMappingService>();

            var employeesId = await employeeTeamMappingService
                .GetAllAttached()
                .Where(x => x.TeamId == teamId)
                .Select(x => x.EmployeeId)
                .ToListAsync();

            decimal moneyForEmployee = 0m;

            foreach ((Guid id, decimal quantity) in jobs)
            {
                var job = await jobService.GetByIdAsync(id.ToString());
                var totalMoney = job.Price * quantity;
                moneyForEmployee += totalMoney / employeesId.Count;
            }
            
            var employeeService = serviceProvider.GetRequiredService<IEmployeeService>();

            var employees = employeeService.GetAllAttached().Where(x => employeesId.Contains(x.Id));

            if (isAdding && !isRemoving)
            {
                foreach ((Guid jobId , decimal quantity) in jobs)
                {
                    if (quantity != 0)
                    {
                        await jobDoneJobService.AddAsync(jobDoneId, jobId, quantity);
                    }
                }
                foreach (var employee in employees)
                {
                    var takenMoneyFromEmployee = employee.Wages * daysForJob;
                    var moneyToTake = moneyForEmployee - takenMoneyFromEmployee;
                    employee.MoneyToTake += moneyToTake;
                }
            }
            else if (isRemoving && !isAdding)
            {
                foreach ((Guid jobId, decimal quantity) in jobs)
                {
                    await jobDoneJobService.RemoveAsync(jobDoneId, jobId);
                }
                foreach (var employee in employees)
                {
                    var takenMoneyFromEmployee = employee.Wages * daysForJob;
                    var moneyToTake = moneyForEmployee - takenMoneyFromEmployee;
                    employee.MoneyToTake -= moneyToTake;
                }
            }
            else
            {
                throw new AggregateException("invalid action parameter");
            }
            return await employeeService.SaveChangesAsync();
        }
    }
}
