using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Infrastructure.Repositories;

namespace SupremeCourt.Infrastructure
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<GameDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>(); // Registrace PlayerRepository
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IWaitingRoomRepository, WaitingRoomRepository>(); // ✅ Přidáno
            return services;
        }
    }

}