﻿using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Application.Common.Interfaces;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Infrastructure.Interfaces;
using SupremeCourt.Infrastructure.Repositories;
using SupremeCourt.Infrastructure.Services;
using SupremeCourt.Infrastructure.SignalR;

namespace SupremeCourt.Infrastructure
{
    public static class RegistrInfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<ISignalRSender, SignalRSender>();
            services.AddSingleton<HubLoggerFilter>();

            services.AddSignalR(options =>
            {
                options.AddFilter<HubLoggerFilter>();
            });
            services.AddScoped<IAIPlayerFactory, AiPlayerFactory>();
            services.AddDbContext<GameDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>(); // Registrace PlayerRepository
            services.AddScoped<IGameRepository, GameRepository>();
            
            services.AddSingleton<IWaitingRoomNotifier, WaitingRoomSignalNotifier>();
            services.AddSingleton<IWaitingRoomListNotifier, WaitingRoomListNotifier>(); // ✅ Registrace Notifieru
            services.AddSingleton<IUserSessionRepository, UserSessionRepository>();
            services.AddHttpClient<IOpenAiGameStrategyService, OpenAiGameStrategyService>();

            services.AddSignalR();
            return services;
        }
    }

}