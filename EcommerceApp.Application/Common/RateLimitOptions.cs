namespace EcommerceApp.Application.Common
{
    public static class RateLimitOptions
    {
        public const string Fixed = "Fixed";
        public const string Sliding = "Sliding";
        public const string Token = "Token";
        public const string Concurrency = "Concurrency";
        public static readonly string[] AllPolicies = { Fixed, Sliding, Token, Concurrency };
    }
}
