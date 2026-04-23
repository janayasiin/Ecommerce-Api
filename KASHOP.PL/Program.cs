
using KASHOP.BLL.Mapping;
using KASHOP.BLL.Service;
using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using KASHOP.DAL.utils;
using KASHOP.PL.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;


namespace KASHOP.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
         
         
         



            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDatabaseServices(builder.Configuration);

            builder.Services.AddIdentityServices();
            builder.Services.AddAuthenticationServices(builder.Configuration);
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddLocalizationServices();

            builder.Services.AddCorsPolicyServices();

            MapsterConfig.MapsterConfigRegister();

            var app = builder.Build();
            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);


           

                app.MapOpenApi();
            app.MapScalarApiReference();

            app.UseCors(CorsPolicyExtensions.PolicyName);

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();   

            app.MapControllers();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var seeders =services.GetServices<ISeedData>();
                foreach (var seeder in seeders) 
                    {
                    try
                    {
                        await seeder.DataSeed();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

            }
            app.Run();

        }
    }
}
