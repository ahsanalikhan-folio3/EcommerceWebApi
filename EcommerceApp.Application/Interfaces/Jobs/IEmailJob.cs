using EcommerceApp.Application.Dtos;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Jobs
{
    public interface IEmailJob
    {
        Task SendOtpEmail(string email, string otp, int expiryLimitInMinutes);
        Task SendWelcomeEmailToCustomer(string email);
        Task SendAccountReviewEmailToSellerOnRegistration(string email);
        Task SendAccountActivationEmail(string email);
        Task SendAccountDeactivationEmail(string email);
        Task SendSuccessfullOrderCompletionEmail(string email, decimal totalAmount, List<OrderDetailsEmailDto> orderDetailsEmailDtos);
        Task SendOrderStatusUpdateEmail(string email, int sellerOrderId, OrderStatus status);
        Task SendOrderCancellationEmail(string email, int sellerOrderId, string cancelledBy);
    }
}
