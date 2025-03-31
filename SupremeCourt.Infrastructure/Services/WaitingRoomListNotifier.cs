using Microsoft.AspNetCore.SignalR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Infrastructure.SignalR;

namespace SupremeCourt.Infrastructure.Services
{
    public class WaitingRoomListNotifier : IWaitingRoomListNotifier
    {
        private readonly IHubContext<WaitingRoomListHub> _hubContext;

        public WaitingRoomListNotifier(IHubContext<WaitingRoomListHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyPlayerJoinedAsync(int gameId, string playerName)
        {
            await _hubContext.Clients.Group(gameId.ToString()).SendAsync("PlayerJoined", playerName);
        }

        public async Task NotifyWaitingRoomCreatedAsync(WaitingRoomDto roomDto)
        {
            await _hubContext.Clients.Group("waitingroom-list").SendAsync("NewWaitingRoomCreated", roomDto);
        }
    }
}