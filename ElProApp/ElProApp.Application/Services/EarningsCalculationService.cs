namespace ElProApp.Application.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using ElProApp.Application.Services.Interfaces;

    using static ElProApp.Common.EntityValidationConstants.CalculationAction;

    /// <summary>
    /// Provides operations for calculating team earnings based on consumed materials.
    /// </summary>
    public class EarningsCalculationService : IEarningsCalculationService
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="EarningsCalculationService"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider for resolving dependencies.</param>
        public EarningsCalculationService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Calculates and applies earnings for a team based on a job-done record.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="jobDoneId">The job-done identifier.</param>
        /// <param name="daysForJob">The number of working days.</param>
        /// <param name="action">The calculation action (Add or Remove).</param>
        /// <returns>True if the calculation was successful.</returns>
        public async Task<bool> CalculateMoneyAsync(
            Guid teamId,
            Guid jobDoneId,
            int daysForJob,
            string action)
        {
            bool isAdding = action == Adding;

            var employeeTeamMappingService =
                serviceProvider.GetRequiredService<IEmployeeTeamMappingService>();

            var employeeService =
                serviceProvider.GetRequiredService<IEmployeeService>();

            var materialMappingService =
                serviceProvider.GetRequiredService<IJobDoneMaterialMappingService>();

            var employees = await employeeTeamMappingService
                .GetAllAttached()
                .Where(x => x.TeamId == teamId)
                .Select(x => x.Employee)
                .ToListAsync();

            int peopleCount = employees.Count;

            if (peopleCount == 0)
                return false;

            var materials = await materialMappingService
                .GetAllAttached()
                .Where(x => x.JobDoneId == jobDoneId)
                .ToListAsync();

            decimal totalMoney = 0m;

            foreach (var m in materials)
            {
                totalMoney += m.Quantity * m.UnitPrice;
            }

            decimal moneyPerEmployee = totalMoney / peopleCount;

            foreach (var e in employees)
            {
                var profit = moneyPerEmployee - (e.Wages * daysForJob);

                if (isAdding)
                    e.MoneyToTake += profit;
                else
                    e.MoneyToTake -= profit;
            }

            return await employeeService.SaveChangesAsync();
        }
    }
}