namespace EcommerceApp.Application.Interfaces.JobServices
{
    public interface IBackgroundJobService
    {
        public void EnqueueCustomerWelcomeEmailJob(string email);
    }
}
