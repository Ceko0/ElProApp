using ElProApp.Web.Models.JobDone;

namespace ElProApp.Services.Data.Interfaces
{
    public interface IEarningsCalculationService
    {
        public Task<bool> CalculateMoneyAsync(JobDoneInputModel model);
    }
}
