using KASHOP.BLL.Service;
using KASHOP.DAL.Repository;
using KASHOP.DAL.utils;
using Stripe;
using System.Reflection.Metadata.Ecma335;

namespace KASHOP.PL.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services , IConfiguration Configuration)
        {

           Services.AddScoped<ICategoryRepository, CategoryRepository>();
           Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<IAuthenticationService, AuthenticationService>();
           Services.AddScoped<ISeedData, RoleSeedData>();
            Services.AddTransient<IEmailSender, EmailSender>();
            Services.AddScoped<IFileService, BLL.Service.FileService>();
            Services.AddScoped<IProductService, BLL.Service.ProductService>();
            Services.AddScoped<IProductRepository, ProductRepository>();
            Services.AddScoped<IBrandRepository, BrandRepository>();
           Services.AddScoped<IBrandService, BrandService>();
            Services.AddScoped<ICartRepository, CartRepository>();
            Services.AddScoped<ICartService, CartService>();
            Services.AddScoped<ICheckoutService ,BLL.Service.CheckoutService>();
            Services.AddScoped<IOrderRepository, OrderRepository>();
           Services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey =Configuration["Stripe:SecretKey"];
            return Services;

        }
        
    }
}