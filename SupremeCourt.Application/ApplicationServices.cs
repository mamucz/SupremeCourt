using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Application.CQRS.Games.Commands;
using SupremeCourt.Application.CQRS.Players.Commands;
using SupremeCourt.Application.Services;
using SupremeCourt.Application.CQRS.WaitingRooms.Queries;
using SupremeCourt.Domain.Interfaces;
using System.Reflection;
using SupremeCourt.Domain.Sessions;
using SupremeCourt.Application.EventHandlers;
using SupremeCourt.Application.Sessions;

namespace SupremeCourt.Application
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            //services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetWaitingRoomsQuery).Assembly));

            // Registrace handlerů a služeb Application vrstvy
            services.AddScoped<CreatePlayerHandler>();
            services.AddScoped<CreateGameHandler>();

            // Registrace služeb
            services.AddScoped<IAuthService, AuthService>();            // ✅ OPRAVA zde
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IWaitingRoomListService, WaitingRoomListService>();
            services.AddSingleton<IWaitingRoomEventHandler, WaitingRoomEventHandler>();
            services.AddSingleton<WaitingRoomSessionManager>();

            services.AddSingleton<TokenBlacklistService>();
            services.AddSingleton<WaitingRoomSession>();
            services.AddScoped<ICreateGameHandler, CreateGameHandler>();
            return services;
        }
    }
}