using Microsoft.AspNetCore.SignalR;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Infrastructure.SignalR;

namespace SupremeCourt.Infrastructure.Services
{
    public class WaitingRoomNotifier : IWaitingRoomNotifier
    {
        private readonly IHubContext<WaitingRoomHub> _hubContext;

        public WaitingRoomNotifier(IHubContext<WaitingRoomHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyPlayerJoinedAsync(int gameId, string playerName)
        {
            await _hubContext.Clients.Group(gameId.ToString()).SendAsync("PlayerJoined", playerName);
        }

        public async Task NotifyWaitingRoomCreatedAsync(object roomDto)
        {
            await _hubContext.Clients.Group("waitingroom-list").SendAsync("NewWaitingRoomCreated", roomDto);
        }
    }
}