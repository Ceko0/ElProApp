namespace ElProApp.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using ElProApp.Application.Services.Interfaces;
    using static ElProApp.Common.EntityValidationConstants.CalculationAction;

    public class EarningsCalculationService : IEarningsCalculationService
    {
        private readonly IServiceProvider serviceProvider;

        public EarningsCalculationService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<bool> CalculateMoneyAsync(
    Guid teamId,
    Dictionary<Guid, decimal> jobs,
    Guid jobDoneId,
    int daysForJob,
    string action)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentException("Action cannot be null or empty.", nameof(action));
            }

            bool isAdding = action == Add;
            bool isRemoving = action == Remove;

            if (isAdding == isRemoving)
            {
                throw new ArgumentOutOfRangeException(nameof(action), "Invalid action parameter.");
            }

            var employeeTeamMappingService =
                serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();

            var jobService =
                serviceProvider.GetRequiredService<IJobService>();

            var jobDoneJobService =
                serviceProvider.GetRequiredService<IJobDoneJobMappingService>();

            var employeeService =
                serviceProvider.GetRequiredService<IEmployeeService>();

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

            var employees = employeeService
                .GetAllAttached()
                .Where(x => employeesId.Contains(x.Id));

            decimal CalculateMoneyToTake(decimal wages)
            {
                var takenMoneyFromEmployee = wages * daysForJob;
                return moneyForEmployee - takenMoneyFromEmployee;
            }

            if (isAdding)
            {
                foreach ((Guid jobId, decimal quantity) in jobs)
                {
                    if (quantity != 0)
                    {
                        if (!jobDoneJobService
                                .GetAllAttached()
                                .Any(x => x.JobDoneId == jobDoneId && x.JobId == jobId))
                        {
                            await jobDoneJobService.AddAsync(jobDoneId, jobId, quantity);
                        }
                    }
                }

                foreach (var employee in employees)
                {
                    employee.MoneyToTake += CalculateMoneyToTake(employee.Wages);
                }
            }
            else
            {
                foreach ((Guid jobId, decimal quantity) in jobs)
                {
                    await jobDoneJobService.RemoveAsync(jobDoneId, jobId);
                }

                foreach (var employee in employees)
                {
                    employee.MoneyToTake -= CalculateMoneyToTake(employee.Wages);
                }
            }

            return await employeeService.SaveChangesAsync();
        }
    }
}
