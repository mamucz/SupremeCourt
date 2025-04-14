using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Application.CQRS.Games.Commands;
using SupremeCourt.Application.EventHandlers;
using SupremeCourt.Application.Services;
using SupremeCourt.Application.Sessions;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Mappings;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // ✅ Registrace MediatR handlerů + pipeline behavior
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(LoggingBehavior<,>).Assembly);
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddSingleton<WaitingRoomMapper>();
        // ✅ Aplikační služby
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IWaitingRoomListService, WaitingRoomListService>();
        services.AddScoped<ICreateGameHandler, CreateGameHandler>();
        services.AddScoped<IWaitingRoomService, WaitingRoomService>();

        // 🔁 Session management
        services.AddSingleton<WaitingRoomSessionManager>();
        services.AddSingleton(provider =>
            new Lazy<WaitingRoomSessionManager>(() => provider.GetRequiredService<WaitingRoomSessionManager>())
        );

        // ✅ Event handler using Lazy to break circular dependency
        services.AddSingleton<IWaitingRoomEventHandler, WaitingRoomEventHandler>();

        // 🛡️ Token blacklist jako singleton
        services.AddSingleton<TokenBlacklistService>();

        return services;
    }
}