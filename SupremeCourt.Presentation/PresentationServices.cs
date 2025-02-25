using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;


namespace SupremeCourt.Presentation
{
    public static class PresentationServices
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            // Registrace SignalR hubu
            services.AddSignalR();

            // Registrace controllerů
            services.AddControllers();

            return services;
        }
    }
}