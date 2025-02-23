using Microsoft.Extensions.DependencyInjection;

namespace SupremeCourt.Application
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registrace handlerů a služeb Application vrstvy
            services.AddScoped<Players.Commands.CreatePlayerHandler>();

            return services;
        }
    }
}