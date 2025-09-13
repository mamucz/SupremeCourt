using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Application.Behaviors;
using SupremeCourt.Application.CQRS.Games.Commands;
using SupremeCourt.Application.EventHandlers;
using SupremeCourt.Application.Services;
using SupremeCourt.Domain.Interfaces;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<RegistrAIPlayersService>();
        services.AddHostedService<AiPlayerRegistrationHostedService>();

        // 📦 MediatR – registrace handlerů
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // 📋 Logging behavior pro CQRS pipeline
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // ✅ Běžné aplikační služby
        services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<ICreateGameHandler, CreateGameHandler>();
       

        // 🧠 Čekací místnosti – singletony pro zachování runtime stavu
        services.AddSingleton<IWaitingRoomSessionManager, WaitingRoomSessionManager>();
        services.AddSingleton<IWaitingRoomEventHandler, WaitingRoomEventHandler>();

        // 📋 Služba seznamu místností zůstává Scoped (obsahuje repozitáře)
        services.AddScoped<IWaitingRoomListService, WaitingRoomListService>();

        // 🔒 Token blacklist – singleton
        services.AddSingleton<TokenBlacklistService>();

        return services;
    }
}