namespace ElProApp.Application.Services.Interfaces
{
    using ElProApp.Web.Models.JobDone;
    public interface IEarningsCalculationService
    {
        Task<bool> CalculateMoneyAsync(Guid teamId, Dictionary<Guid, decimal> jobs, Guid jobDoneId, int daysForJob, string action);
    }
}
