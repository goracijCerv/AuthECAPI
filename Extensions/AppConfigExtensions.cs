using AuthECAPI.Models;

namespace AuthECAPI.Extensions
{
    public static class AppConfigExtensions
    {
        public static WebApplication ConfigCors (this WebApplication app)
        {
            app.UseCors(opt => opt.WithOrigins("http://localhost:4200/").AllowAnyMethod().AllowAnyHeader());
            return app;
        }

        public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration config) {
            //Esto es una injeción de dependencias pero solamente a las appsettings
            services.Configure<AppSettings>(config.GetSection("AppSettings"));
            return services;
        }
    }
}
