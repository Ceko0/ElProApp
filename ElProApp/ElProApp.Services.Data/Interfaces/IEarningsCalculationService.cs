using ElProApp.Web.Models.JobDone;

namespace ElProApp.Services.Data.Interfaces
{
    public interface IEarningsCalculationService
    {
        Task<bool> CalculateMoneyAsync(Guid teamId, Dictionary<Guid, decimal> jobs, Guid jobDoneId, int daysForJob, string action);
    }
}
