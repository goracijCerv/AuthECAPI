using Microsoft.EntityFrameworkCore;

namespace AuthECAPI.Extensions
{
    public static class EntityFExtensions
    {
        public static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration config){
            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            return services;

        }
    }
}
