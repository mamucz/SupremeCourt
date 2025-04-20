using Microsoft.AspNetCore.SignalR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Infrastructure.SignalR;

namespace SupremeCourt.Infrastructure.Services
{
    public class WaitingRoomNotifier : IWaitingRoomNotifier
    {
        private readonly IHubContext<WaitingRoomListHub> _listHub;
        private readonly IHubContext<WaitingRoomHub> _roomHub;

        public WaitingRoomNotifier(
            IHubContext<WaitingRoomListHub> listHub,
            IHubContext<WaitingRoomHub> roomHub)
        {
            _listHub = listHub;
            _roomHub = roomHub;
        }

        public async Task NotifyPlayerJoinedAsync(int waitingRoomId, string playerName)
        {
            await _roomHub.Clients.Group(waitingRoomId.ToString())
                .SendAsync("PlayerJoined", playerName);
        }

        public async Task NotifyWaitingRoomCreatedAsync(object dto)
        {
            await _listHub.Clients.All.SendAsync("NewWaitingRoomCreated", dto);
        }

        public async Task NotifyCountdownTickAsync(int roomId, int secondsLeft)
        {
            // 👥 1️⃣ Poslat do přehledu místností
            await _listHub.Clients.Group("waitingroom-list")
                .SendAsync("UpdateTimeLeft", new
                {
                    waitingRoomId = roomId,
                    timeLeftSeconds = secondsLeft
                });

            // 🧍 2️⃣ Poslat do konkrétní místnosti (detail místnosti)
            await _roomHub.Clients.Group(roomId.ToString())
                .SendAsync("CountdownTick", secondsLeft);
        }

        public async Task NotifyRoomExpiredAsync(int roomId)
        {
            await _roomHub.Clients.Group(roomId.ToString())
                .SendAsync("RoomExpired");
        }

        public async Task NotifyRoomUpdatedAsync(WaitingRoomDto dto)
        {
            await _roomHub.Clients.Group(dto.WaitingRoomId.ToString())
                .SendAsync("RoomUpdated", dto);
        }
    }
}
