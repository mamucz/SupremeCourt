using Microsoft.AspNetCore.SignalR;
using SupremeCourt.Domain.DTOs;
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

        public async Task NotifyRoomUpdatedAsync(WaitingRoomDto dto)
        {
            await _hubContext.Clients.Group(dto.WaitingRoomId.ToString())
                .SendAsync("RoomUpdated", dto);
        }

        public async Task NotifyCountdownAsync(int roomId, int secondsLeft)
        {
            await _hubContext.Clients.Group(roomId.ToString())
                .SendAsync("CountdownTick", secondsLeft);
        }

        public async Task NotifyRoomExpiredAsync(int roomId)
        {
            await _hubContext.Clients.Group(roomId.ToString())
                .SendAsync("RoomExpired");
        }

        public async Task NotifyWaitingRoomCreatedAsync(object payload)
        {
            await _hubContext.Clients.All.SendAsync("NewWaitingRoomCreated", payload);
        }

        public async Task NotifyPlayerJoinedAsync(int waitingRoomId, string playerName)
        {
            await _hubContext.Clients.Group(waitingRoomId.ToString())
                .SendAsync("PlayerJoined", playerName);
        }

        public async Task NotifyCountdownTickAsync(int roomId, int secondsLeft)
        {
            await _hubContext.Clients.Group(roomId.ToString())
                .SendAsync("CountdownTick", secondsLeft);
        }
    }
}