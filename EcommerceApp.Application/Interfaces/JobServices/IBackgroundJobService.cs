using EcommerceApp.Application.Dtos;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.JobServices
{
    public interface IBackgroundJobService
    {
        public void EnqueueOtpEmailJob(string email, string otp, int expiryLimitInMinutes);
        public void EnqueueCustomerWelcomeEmailJob(string email);
        public void EnqueueSuccessfullOrderCompletionEmailJob(string email, decimal totalAmount, List<OrderDetailsEmailDto> orderDetailsEmailDtos);
        public void EnqueueAccountActivationEmailJob(string email);
        public void EnqueueAccountDeactivationEmailJob(string email);
        public void EnqueueAccountReviewOfSellerOnRegistrationEmailJob(string email);
        public void EnqueueOrderStatusUpdateEmailJob(string email, int sellerOrderId, OrderStatus orderStatus);
        public void EnqueueOrderCancellationEmailJob(string email, int sellerOrderId, string cancelledBy);
    }
}
