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
            services.AddScoped<CreateGameHandler>();

            // Registrace služeb
            services.AddScoped<IAuthService, AuthService>();            // ✅ OPRAVA zde
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IWaitingRoomService, WaitingRoomService>();
            services.AddSingleton<TokenBlacklistService>();
            services.AddScoped<ICreateGameHandler, CreateGameHandler>();
            return services;
        }
    }
}