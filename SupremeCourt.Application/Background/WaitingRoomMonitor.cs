using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using SupremeCourt.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using SupremeCourt.Domain.Logic;


namespace SupremeCourt.Application.Background
{
    public class WaitingRoomMonitor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WaitingRoomMonitor> _logger;
        private readonly int _expirationMinutes;

        public WaitingRoomMonitor(IServiceScopeFactory scopeFactory, ILogger<WaitingRoomMonitor> logger, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _expirationMinutes = _expirationMinutes = configuration.GetValue<int>("WaitingRoom:ExpirationMinutes", 15);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var waitingRoomService = scope.ServiceProvider.GetRequiredService<IWaitingRoomService>();
                var waitingRoomRepository = scope.ServiceProvider.GetRequiredService<IWaitingRoomRepository>();

                var allWaitingRooms = await waitingRoomService.GetAllWaitingRoomsAsync();

                foreach (var waitingRoom in allWaitingRooms)
                {
                    var isExpired = DateTime.UtcNow > waitingRoom.CreatedAt.AddMinutes(_expirationMinutes);
                    var hasEnoughPlayers = waitingRoom.Players.Count >= GameRules.MaxPlayers;

                    if (isExpired && !hasEnoughPlayers)
                    {
                        await waitingRoomRepository.DeleteAsync(waitingRoom);
                        _logger.LogInformation("🗑️ WaitingRoom {Id} byl zrušen – expiroval a nemá dost hráčů.", waitingRoom.Id);
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}