using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Jobs;
using EcommerceApp.Application.Interfaces.JobServices;
using EcommerceApp.Domain.Entities;
using Hangfire;

namespace EcommerceApp.Infrastructure.JobService
{
    public class HangfireBackgroundJobService : IBackgroundJobService
    {
        public void EnqueueCustomerWelcomeEmailJob(string email)
        {
            BackgroundJob.Enqueue<IEmailJob>(job => job.SendWelcomeEmailToCustomer(email));
        }
        public void EnqueueSuccessfullOrderCompletionEmailJob(string email, decimal totalAmount, List<OrderDetailsEmailDto> orderDetailsEmailDtos)
        {
            BackgroundJob.Enqueue<IEmailJob>(job => job.SendSuccessfullOrderCompletionEmail(email, totalAmount, orderDetailsEmailDtos));
        }
        public void EnqueueAccountActivationEmailJob(string email)
        {
            BackgroundJob.Enqueue<IEmailJob>(job => job.SendAccountActivationEmail(email));
        }
        public void EnqueueAccountDeactivationEmailJob(string email)
        {
            BackgroundJob.Enqueue<IEmailJob>(job => job.SendAccountDeactivationEmail(email));
        }
        public void EnqueueAccountReviewOfSellerOnRegistrationEmailJob(string email)
        {
            BackgroundJob.Enqueue<IEmailJob>(job => job.SendAccountReviewEmailToSellerOnRegistration(email));
        }
        public void EnqueueOrderStatusUpdateEmailJob(string email, int sellerOrderId, OrderStatus orderStatus)
        {
            BackgroundJob.Enqueue<IEmailJob>(job => job.SendOrderStatusUpdateEmail(email, sellerOrderId, orderStatus));
        }

        public void EnqueueOrderCancellationEmailJob(string email, int sellerOrderId, string cancelledBy)
        {
            BackgroundJob.Enqueue<IEmailJob>(job => job.SendOrderCancellationEmail(email, sellerOrderId, cancelledBy));
        }
    }
}
