using EcommerceApp.Application.Common;
using EcommerceApp.Application.Interfaces.Jobs;
using EcommerceApp.Domain.Entities;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EcommerceApp.Infrastructure.Jobs
{
    public class EmailJob : IEmailJob
    {
        private readonly EmailSettings emailSettings;
        public EmailJob(IOptions<EmailSettings> options)
        {
            this.emailSettings = options.Value;
        }
        private async Task SendEmail (MimeMessage message)
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emailSettings.Username, emailSettings.AppPassword);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
        private MimeMessage GetMimeMessage (string email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailSettings.FromName, emailSettings.FromEmail));
            message.To.Add(new MailboxAddress("", email));

            return message;
        }
        public Task SendAccountActivationEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task SendAccountDeactivationEmail(string email)
        {
            throw new NotImplementedException();
        }

        public async Task SendWelcomeEmailToCustomer(string email)
        {
            var message = this.GetMimeMessage(email);

            message.Subject = "Welcome to Our Ecommerce App 🎉";

            message.Body = new TextPart("html")
            {
                Text = @"
        <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2>Welcome aboard! 👋</h2>

            <p>Thank you for registering with <strong>Our Ecommerce App</strong>.</p>

            <p>Your account has been successfully created. You can now:</p>

            <ul>
                <li>Browse thousands of products</li>
                <li>Track your orders in real time</li>
                <li>Manage your profile and preferences</li>
            </ul>

            <p>
                If you have any questions, feel free to reply to this email — 
                we’re always happy to help.
            </p>

            <p>
                Happy shopping! 🛍️<br/>
                <strong>The Ecommerce Team</strong>
            </p>
        </div>
    "
            };


            await this.SendEmail(message);
        }

        public Task SendOrderStatusUpdateEmail(string email, OrderStatus status)
        {
            throw new NotImplementedException();
        }

        public Task SendSuccessfullOrderCompletionEmail(string email)
        {
            throw new NotImplementedException();
        }
    }
}
