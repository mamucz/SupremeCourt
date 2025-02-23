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
            // Přidáme using pro EF Core SQL Server
            services.AddDbContext<GameDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Registrace repozitářů
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}