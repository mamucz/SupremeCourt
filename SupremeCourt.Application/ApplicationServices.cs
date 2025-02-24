using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Application.Games.Commands;
using SupremeCourt.Application.Services;

namespace SupremeCourt.Application
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registrace handlerů a služeb Application vrstvy
            services.AddScoped<Players.Commands.CreatePlayerHandler>();
            // Registrace AuthService
            services.AddScoped<AuthService>();
            services.AddSingleton<TokenBlacklistService>();
            services.AddScoped<GameService>();
            services.AddScoped<CreateGameHandler>();
            return services;
        }
    }
}