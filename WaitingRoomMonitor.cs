using System.Threading.Tasks;
using System.Threading;
using System;

namespace SupremeCourt.Application.Background
{
    public class WaitingRoomMonitor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WaitingRoomMonitor> _logger;

        public WaitingRoomMonitor(IServiceScopeFactory scopeFactory, ILogger<WaitingRoomMonitor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var waitingRoomService = scope.ServiceProvider.GetRequiredService<WaitingRoomService>();

                var allGames = await waitingRoomService.GetAllWaitingRoomsAsync();
                foreach (var waitingRoom in allGames)
                {
                    if (await waitingRoomService.IsTimeExpiredAsync(waitingRoom.GameId))
                    {
                        _logger.LogInformation($"WaitingRoom {waitingRoom.Id} expired. Starting game.");
                        // Zde bychom zavolali start hry
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

}
