using EcommerceApp.Application.Interfaces.Jobs;
using EcommerceApp.Application.Interfaces.JobServices;
using Hangfire;

namespace EcommerceApp.Infrastructure.JobService
{
    public class HangfireBackgroundJobService : IBackgroundJobService
    {
        public void EnqueueCustomerWelcomeEmailJob(string email)
        {
            BackgroundJob.Enqueue<IEmailJob>(job => job.SendWelcomeEmailToCustomer(email));
        }
    }
}
