namespace KASHOP.PL.Middleware
{

    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UsecUSTOMmIDDLEWARE(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CustomMiddleware>();
        }
        public class CustomMiddleware
        {
            private readonly RequestDelegate _next;
            public CustomMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                Console.WriteLine("Processing Request");
                await _next(context);
                Console.WriteLine("Processing Response");


            }
        }
    }
}
