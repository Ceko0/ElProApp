using ElProApp.Web.Models.JobDone;

namespace ElProApp.Services.Data.Interfaces
{
    public interface IEarningsCalculationService
    {
        Task<bool> CalculateMoneyAsync(JobDoneInputModel model); 
        Task<bool> CalculateDeletingMoneyAsync(JobDoneInputModel model);
    }
}
