using API.Data;
using API.Helpers;
using API.interfaces;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt => {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            services.AddCors();

            // lifetime on how long do we want the services to be available
            /*
            * 
            1. AddTransient() 
                - short live service
                - the token service will be created and disposed within the request as soon as it's used and finished
                - when controller disposed at the end of the HTTP request, any dependent services also disposed
            2. AddSingleton()
                - create service that's instantiated when application first starts
                - never dispose until application close down
            3. AddScoped()
                - Scoped objects are the same within a request, but different across different requests
                - created once per request within the scope
            */
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();

            return services;
        }
    }
}