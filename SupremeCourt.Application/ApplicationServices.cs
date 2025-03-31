using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Application.CQRS.Games.Commands;
using SupremeCourt.Application.EventHandlers;
using SupremeCourt.Application.Services;
using SupremeCourt.Application.Sessions;
using SupremeCourt.Domain.Interfaces;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IWaitingRoomListService, WaitingRoomListService>();
        services.AddScoped<ICreateGameHandler, CreateGameHandler>();

        // 🔁 Session management
        services.AddSingleton<WaitingRoomSessionManager>();
        services.AddSingleton(provider =>
            new Lazy<WaitingRoomSessionManager>(() => provider.GetRequiredService<WaitingRoomSessionManager>())
        );

        // ✅ Event handler using Lazy to break circular dependency
        services.AddSingleton<IWaitingRoomEventHandler, WaitingRoomEventHandler>();

      
        // Token blacklist je taky singleton
        services.AddSingleton<TokenBlacklistService>();

        return services;
    }
}
