using KASHOP.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace KASHOP.PL.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabaseServices( this IServiceCollection Services , IConfiguration Configuration)
        {
            Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            return Services;
        }
    }
}
