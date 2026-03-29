namespace ElProApp.Application.Services.Interfaces
{
    using ElProApp.Web.Models.JobDone;
    public interface IEarningsCalculationService
    {
        Task<bool> CalculateMoneyAsync(Guid teamId, Guid jobDoneId, int daysForJob, string action);
    }
}
