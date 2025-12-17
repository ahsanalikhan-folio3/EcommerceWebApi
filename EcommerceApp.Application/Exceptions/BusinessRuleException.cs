namespace EcommerceApp.Application.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string Message) : base(Message) {}
    }
}
