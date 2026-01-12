namespace EcommerceApp.Application.Common
{
    public class EmailSettings
    {
        public required string Host { get; set; }
        public required int Port { get; set; }
        public required string Username { get; set; }
        public required string AppPassword { get;   set; }
        public required bool EnableSsl { get; set; }
        public required string FromName { get; set; }
        public required string FromEmail { get; set; }
    }
}
