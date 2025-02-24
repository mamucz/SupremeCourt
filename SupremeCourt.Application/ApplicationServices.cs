using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Application.Games.Commands;
using SupremeCourt.Application.Services;
using SupremeCourt.Domain.Interfaces;

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
            services.AddScoped<CreateGameHandler>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IWaitingRoomService, WaitingRoomService>(); // ✅ Přidáno
            return services;
        }
    }
}