namespace EcommerceApp.Application.Common
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";
        public const string CustomerService = "CustomerService";
        public const string Seller = "Seller";
        public static readonly string[] AllRoles = {Admin, Customer, CustomerService, Seller};
    }
}
