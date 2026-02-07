using EcommerceApp.Application.Common;
using EcommerceApp.Application.Interfaces.Jobs;
using EcommerceApp.Domain.Entities;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Infrastructure.Jobs
{
    public class EmailJob : IEmailJob
    {
        private readonly EmailSettings emailSettings;
        public EmailJob(IOptions<EmailSettings> options)
        {
            this.emailSettings = options.Value;
        }
        private MimeMessage GetMimeMessage(string email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailSettings.FromName, emailSettings.FromEmail));
            message.To.Add(new MailboxAddress("", email));

            return message;
        }
        private async Task SendEmail (MimeMessage message)
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emailSettings.Username, emailSettings.AppPassword);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
        public async Task SendAccountActivationEmail(string email)
        {
            var message = this.GetMimeMessage(email);

            message.Subject = "Welcome to Our Ecommerce App 🎉";
            message.Body = new TextPart("html")
            {
                Text = @"
        <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2>Your Account Has Been Activated 🎉</h2>

            <p>
                We’re happy to let you know that after a careful review by the
                <strong>Our Ecommerce App</strong> team, your account has been
                <strong>successfully approved and activated</strong>.
            </p>

            <p>
                You can now log in and start using all the features available to you
                on our platform.
            </p>

            <p>
                Please make sure your profile information remains accurate and that
                you follow our platform guidelines to continue enjoying uninterrupted
                access.
            </p>

            <p>
                If you have any questions or need assistance, our support team is always
                here to help.
            </p>

            <p>
                Welcome aboard, and we’re glad to have you with us! 🚀<br/>
                <strong>The Ecommerce Team</strong>
            </p>
        </div>
    "
            };

            await this.SendEmail(message);
        }

        public async Task SendAccountDeactivationEmail(string email)
        {
            var message = this.GetMimeMessage(email);

            message.Subject = "Account Deactivation Notice";
            message.Body = new TextPart("html")
            {
                Text = @"
        <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2>Your Account Has Been Deactivated</h2>
            <p>We wanted to inform you that your account with <strong>Our Ecommerce App</strong> has been
            deactivated. If you believe this is a mistake or have any questions, please contact our support team.</p>
            <p>Thank you for being a valued member of our community.</p>
            <p>Best regards,<br/><strong>The Ecommerce Team</strong></p>
        </div>
    "
            };

            await this.SendEmail(message);
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

        public async Task SendOrderStatusUpdateEmail(
            string email,
            int sellerOrderId,
            OrderStatus status)
        {
            var message = this.GetMimeMessage(email);

            string subject = $"Update on Your Order #{sellerOrderId}";

            string bodyText = status switch
            {
                OrderStatus.Processing => $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>

    <h2>Your Order is Being Processed 🛠️</h2>

    <p>Hi,</p>

    <p>Your order <strong>#{sellerOrderId}</strong> is now <strong>Processing</strong>. 
       Product Seller is preparing your items for shipment.</p>

    <p>You will receive another update once your order is shipped.</p>

    <p>Thank you for shopping with <strong>Our Ecommerce App</strong>!</p>

    <p><strong>The Ecommerce Team</strong></p>

</div>",

                OrderStatus.InWarehouse => $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>

    <h2>Your Order is in Our Warehouse 📦</h2>

    <p>Hi,</p>

    <p>Your order <strong>#{sellerOrderId}</strong> has arrived at our warehouse and is being prepared for shipping.</p>

    <p>It will be dispatched soon. We’ll notify you once it’s on the way.</p>

    <p>Thank you for choosing <strong>Our Ecommerce App</strong>!</p>

    <p><strong>The Ecommerce Team</strong></p>

</div>",

                OrderStatus.Shipped => $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>

    <h2>Your Order Has Been Shipped 🚚</h2>

    <p>Hi,</p>

    <p>Your order <strong>#{sellerOrderId}</strong> is now <strong>Shipped</strong> and on its way to you.</p>

    <p>Thank you for shopping with <strong>Our Ecommerce App</strong>!</p>

    <p><strong>The Ecommerce Team</strong></p>

</div>",

                OrderStatus.Delivered => $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>

    <h2>Your Order Has Been Delivered 🎉</h2>

    <p>Hi,</p>

    <p>We’re happy to inform you that your order <strong>#{sellerOrderId}</strong> has been successfully <strong>Delivered</strong>.</p>

    <p>If you have any questions or issues, feel free to contact our support team.</p>

    <p>Thank you for shopping with <strong>Our Ecommerce App</strong>!</p>

    <p><strong>The Ecommerce Team</strong></p>

</div>",
                _ => $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>

    <p>Hi,</p>

    <p>Your order <strong>#{sellerOrderId}</strong> has an updated status: <strong>{status}</strong>.</p>

    <p>Thank you for shopping with <strong>Our Ecommerce App</strong>!</p>

    <p><strong>The Ecommerce Team</strong></p>

</div>"
            };

            message.Subject = subject;
            message.Body = new TextPart("html") { Text = bodyText };

            await this.SendEmail(message);
        }

        public async Task SendSuccessfullOrderCompletionEmail(string email, decimal totalAmount, List<OrderDetailsEmailDto> orderDetailsEmailDtos)
        {
            var message = this.GetMimeMessage(email);

            var rows = string.Join("", orderDetailsEmailDtos.Select(item => $@"
        <tr>
            <td style='padding:8px;border:1px solid #ddd;'>{item.ProductName}</td>
            <td style='padding:8px;border:1px solid #ddd;text-align:center;'>{item.SellerOrderId}</td>
            <td style='padding:8px;border:1px solid #ddd;text-align:center;'>{item.Quantity}</td>
        </tr>
    "));

            message.Subject = "Your Order is Complete! 🎉";
            message.Body = new TextPart("html")
            {
                Text = $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>

    <h2>🎉 Your Order Is Complete!</h2>

    <p>
        Thank you for shopping with <strong>Our Ecommerce App</strong>.
        Your order has been successfully processed.
    </p>

    <h3>Order Details</h3>

    <table style='border-collapse:collapse;width:100%;margin-top:10px;'>
        <thead>
            <tr style='background-color:#f3f4f6;'>
                <th style='padding:8px;border:1px solid #ddd;text-align:left;'>Product Name</th>
                <th style='padding:8px;border:1px solid #ddd;text-align:center;'>Seller Order ID</th>
                <th style='padding:8px;border:1px solid #ddd;text-align:center;'>Quantity</th>
            </tr>
        </thead>
        <tbody>
            {rows}
        </tbody>
        <tfoot>
            <tr>
                <td colspan='2' style='padding:10px;border:1px solid #ddd;text-align:right;'>
                    <strong>Total Amount</strong>
                </td>
                <td style='padding:10px;border:1px solid #ddd;text-align:center;'>
                    <strong>${totalAmount:F2}</strong>
                </td>
            </tr>
        </tfoot>
    </table>

    <p style='margin-top:20px;'>
        You can track your order anytime from your account dashboard.
        As your order is processed, it will move through the following statuses:
    </p>

    <p style='margin-top:10px;'>
        <strong>Pending</strong> → <strong>Processing</strong> → <strong>In Warehouse</strong> →
        <strong>Shipped</strong> → <strong>Delivered</strong>
    </p>

    <p style='margin-top:10px; color:#555; font-size:14px;'>
        We’ll keep you informed at every step, and you’ll receive updates as your order progresses.
    </p>


    <p>
        If you have any questions, our support team is always happy to help.
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

        public async Task SendAccountReviewEmailToSellerOnRegistration(string email)
        {
            var message = this.GetMimeMessage(email);

            message.Subject = "Your Seller Account is Under Review 🛠️";
            message.Body = new TextPart("html")
            {
                Text = @"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>

    <h2>Welcome to Our Ecommerce App! 👋</h2>

    <p>Thank you for registering as a seller with <strong>Our Ecommerce App</strong>.</p>

    <p>
        Your account has been successfully created and is now <strong>under review</strong> by our team.
        We carefully review all seller accounts to ensure a safe and trustworthy marketplace.
    </p>

    <p>
        Once your account has been reviewed and approved, you will receive an email confirming that your account is activated and ready to use.
    </p>

    <p>
        In the meantime, you can review our seller guidelines to prepare your store and listings for a smooth approval process.
    </p>

    <p>
        If you have any questions or need assistance, feel free to reach out to our support team — we’re always happy to help.
    </p>

    <p>
        Thank you for joining our marketplace! 🚀<br/>
        <strong>The Ecommerce Team</strong>
    </p>

</div>
"
            };

            await this.SendEmail(message);
        }
        // User Role defines who cancelled the order, if it's Customer then email goes to seller and if it's Seller then email goes to customer and if it's admin then email goes to both customer and seller.
        public async Task SendOrderCancellationEmail(string email, int sellerOrderId, string cancelledBy)
        {
            var message = this.GetMimeMessage(email);
            
            var subject = cancelledBy switch
            {
                AppRoles.Customer => $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>

    <h2>Order Cancelled ❌</h2>

    <p>Hi,</p>

    <p>We wanted to inform you that the customer has <strong>cancelled</strong> the order 
       <strong>#{sellerOrderId}</strong>.</p>

    <p>Please update your records accordingly and do not proceed with processing this order.</p>

    <p>If you have any questions or need further assistance, feel free to contact our support team.</p>

    <p>Thank you for being a valued partner with <strong>Our Ecommerce App</strong>!</p>

    <p><strong>The Ecommerce Team</strong></p>

</div>"
,
                AppRoles.Seller => $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>

    <h2>Order Cancelled ❌</h2>

    <p>Hi,</p>

    <p>We’re sorry to inform you that the <strong>seller has cancelled</strong> your order 
       <strong>#{sellerOrderId}</strong>.</p>

    <p>Please visit our website and review your order details to see the <strong>cancellation reason provided by the seller</strong>.</p>

    <p>We apologize for any inconvenience this may have caused and appreciate your understanding.</p>

    <p>If you have any questions or need help placing a new order, our support team is always here to assist you.</p>

    <p>Thank you for shopping with <strong>Our Ecommerce App</strong>.</p>

    <p><strong>The Ecommerce Team</strong></p>

</div>"
,
                AppRoles.Admin => $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>

    <h2>Order Cancelled ❌</h2>

    <p>Hi,</p>

    <p>We’re writing to inform you that your order 
       <strong>#{sellerOrderId}</strong> has been <strong>cancelled by the website administrator</strong>.</p>

    <p>Please visit our website and review your order details to see the <strong>cancellation reason provided by the admin</strong>.</p>

    <p>We sincerely apologize for any inconvenience this may have caused and appreciate your understanding.</p>

    <p>If you have any questions or need assistance placing a new order, our support team is always here to help.</p>

    <p>Thank you for shopping with <strong>Our Ecommerce App</strong>.</p>

    <p><strong>The Ecommerce Team</strong></p>

</div>"
,
                _ => $@"Your Order # {sellerOrderId} is cancelled."

            };

            await this.SendEmail(message);
        }
    }
}
