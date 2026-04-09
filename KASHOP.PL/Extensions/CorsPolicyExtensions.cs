namespace KASHOP.PL.Extensions
{
    public static class CorsPolicyExtensions
    {
        public const string  PolicyName = "_myAllowSpecificOrigins";

        public static IServiceCollection AddCorsPolicyServices (this IServiceCollection Services)
        {
           


           Services.AddCors(options =>
            {
                options.AddPolicy(name: PolicyName,
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                                  });
            });
            return Services;
        }
    }
}
