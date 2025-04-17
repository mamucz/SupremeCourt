using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Application.Common.Interfaces;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Infrastructure.Repositories;
using SupremeCourt.Infrastructure.Services;

namespace SupremeCourt.Infrastructure
{
    public static class RegistrInfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<GameDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>(); // Registrace PlayerRepository
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IWaitingRoomRepository, WaitingRoomRepository>(); // ✅ Přidáno
            
            services.AddSingleton<IWaitingRoomNotifier, WaitingRoomNotifier>();
            services.AddSingleton<IWaitingRoomListNotifier, WaitingRoomListNotifier>(); // ✅ Registrace Notifieru
            services.AddSingleton<IUserSessionRepository, UserSessionRepository>();

            services.AddSignalR();
            return services;
        }
    }

}