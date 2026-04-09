using KASHOP.BLL.Service;
using KASHOP.DAL.Repository;
using KASHOP.DAL.utils;
using System.Reflection.Metadata.Ecma335;

namespace KASHOP.PL.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {

           Services.AddScoped<ICategoryRepository, CategoryRepository>();
           Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<IAuthenticationService, AuthenticationService>();
           Services.AddScoped<ISeedData, RoleSeedData>();
            Services.AddTransient<IEmailSender, EmailSender>();
            Services.AddScoped<IFileService, FileService>();
            Services.AddScoped<IProductService, ProductService>();
            Services.AddScoped<IProductRepository, ProductRepository>();
            Services.AddScoped<IBrandRepository, BrandRepository>();
           Services.AddScoped<IBrandService, BrandService>();
            Services.AddScoped<ICartRepository, CartRepository>();
            Services.AddScoped<ICartService, CartService>();
            return Services;

        }
        
    }
}