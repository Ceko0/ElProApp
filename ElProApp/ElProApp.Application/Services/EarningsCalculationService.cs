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

        public async Task<bool> CalculateMoneyAsync(Guid teamId,
                                            Dictionary<Guid, decimal> jobs,
                                            Guid jobDoneId,
                                            int daysForJob,
                                            string action)
        {
            bool isAdding = action == Add;

            var employeeTeamMappingService =
                serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();

            var employeeService =
                serviceProvider.GetRequiredService<IEmployeeService>();

            var jobDoneJobService =
                serviceProvider.GetRequiredService<IJobDoneJobMappingService>();

            var employees = await employeeTeamMappingService
                .GetAllAttached()
                .Where(x => x.TeamId == teamId)
                .Select(x => x.Employee)
                .ToListAsync();

            int peopleCount = employees.Count;

            if (peopleCount == 0)
                return false;

            if (isAdding)
            {
                var existing = await jobDoneJobService
                    .GetAllAttached()
                    .Where(x => x.JobDoneId == jobDoneId)
                    .ToListAsync();

                foreach (var m in existing)
                {
                    await jobDoneJobService.RemoveAsync(m.JobDoneId, m.JobId);
                }

                foreach ((Guid jobId, decimal quantity) in jobs)
                {
                    if (quantity > 0)
                    {
                        await jobDoneJobService.AddAsync(jobDoneId, jobId, quantity);
                    }
                }
            }

            var mappings = await jobDoneJobService
                .GetAllAttached()
                .Where(x => x.JobDoneId == jobDoneId)
                .ToListAsync();

            decimal totalMoney = 0m;

            foreach (var mapping in mappings)
            {
                totalMoney += mapping.Job.Price * mapping.Quantity;
            }

            decimal moneyPerEmployee = totalMoney / peopleCount;

            if (isAdding)
            {
                foreach (var e in employees)
                {
                    var profit = moneyPerEmployee - (e.Wages * daysForJob);
                    e.MoneyToTake += profit;
                }
            }
            else
            {
                foreach (var e in employees)
                {
                    var profit = moneyPerEmployee - (e.Wages * daysForJob);
                    e.MoneyToTake -= profit;
                }
            }

            return await employeeService.SaveChangesAsync();
        }
    }
}
