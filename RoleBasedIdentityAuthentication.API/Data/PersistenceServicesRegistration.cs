using Microsoft.EntityFrameworkCore;
namespace RoleBasedIdentityAuthentication.API.Data


{
    public static class PersistenceServicesRegistration
    {
        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<ApplicationDbSeeder>();
            return services;
        }
    }
}
