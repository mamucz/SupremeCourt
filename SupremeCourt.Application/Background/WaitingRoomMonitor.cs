using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using SupremeCourt.Domain.Interfaces;

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
                var waitingRoomService = scope.ServiceProvider.GetRequiredService<IWaitingRoomService>();

                var allWaitingRooms = await waitingRoomService.GetAllWaitingRoomsAsync();
                foreach (var waitingRoom in allWaitingRooms)
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