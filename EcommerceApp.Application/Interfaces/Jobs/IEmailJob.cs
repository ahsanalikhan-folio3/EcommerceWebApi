using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Jobs
{
    public interface IEmailJob
    {
        Task SendWelcomeEmailToCustomer(string email);
        Task SendAccountActivationEmail(string email);
        Task SendAccountDeactivationEmail(string email);
        Task SendSuccessfullOrderCompletionEmail(string email);
        Task SendOrderStatusUpdateEmail(string email, OrderStatus status);
    }
}
