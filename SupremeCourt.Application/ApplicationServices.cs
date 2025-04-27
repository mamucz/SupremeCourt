using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Application.CQRS.Games.Commands;
using SupremeCourt.Application.EventHandlers;
using SupremeCourt.Application.Services;
using SupremeCourt.Application.Behaviors; // ⬅️ pokud je LoggingBehavior zde
using SupremeCourt.Domain.Interfaces;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // ✅ Aplikační služby
        services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<ICreateGameHandler, CreateGameHandler>();

        services.AddScoped<IWaitingRoomSessionManager, WaitingRoomSessionManager>();
        services.AddScoped<IWaitingRoomListService, WaitingRoomListService>();
        services.AddScoped<IWaitingRoomEventHandler, WaitingRoomEventHandler>();

        // 🔥 Přidání Lazy<IWaitingRoomSessionManager>
        services.AddScoped(provider =>
            new Lazy<IWaitingRoomSessionManager>(() => provider.GetRequiredService<IWaitingRoomSessionManager>())
        );

        // ✅ Token blacklist jako singleton
        services.AddSingleton<TokenBlacklistService>();

        return services;
    }
}