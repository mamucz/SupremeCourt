using Microsoft.AspNetCore.SignalR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Infrastructure.Interfaces;
using SupremeCourt.Infrastructure.SignalR;

namespace SupremeCourt.Infrastructure.Services
{
    public class WaitingRoomNotifier : IWaitingRoomNotifier
    {
        private readonly ISignalRSender _signalRSender;
        private readonly IHubContext<WaitingRoomListHub> _listHub;
        private readonly IHubContext<WaitingRoomHub> _roomHub;

        public WaitingRoomNotifier(
            ISignalRSender signalRSender,
            IHubContext<WaitingRoomListHub> listHub,
            IHubContext<WaitingRoomHub> roomHub)
        {
            _signalRSender = signalRSender;
            _listHub = listHub;
            _roomHub = roomHub;
        }

        public Task NotifyPlayerJoinedAsync(Guid waitingRoomId, PlayerDto player)
        {
            return _signalRSender.SendToGroupAsync(_roomHub, waitingRoomId.ToString(), "PlayerJoined", player);
        }

        public Task NotifyWaitingRoomCreatedAsync(object dto)
        {
            return _signalRSender.SendToAllAsync(_listHub, "NewWaitingRoomCreated", dto);
        }

        public async Task NotifyCountdownTickAsync(Guid roomId, int secondsLeft)
        {
            // 👥 1️⃣ Update v seznamu místností
            await _signalRSender.SendToGroupAsync(_listHub, "waitingroom-list", "UpdateTimeLeft", new
            {
                waitingRoomId = roomId,
                timeLeftSeconds = secondsLeft
            });

            // 🧍 2️⃣ Update v konkrétní místnosti
            await _signalRSender.SendToGroupAsync(_roomHub, roomId.ToString(), "CountdownTick", secondsLeft);
        }

        public async Task NotifyRoomExpiredAsync(Guid roomId)
        {
            // 📤 Do konkrétní místnosti (detail)
            await _signalRSender.SendToGroupAsync(_roomHub, roomId.ToString(), "RoomExpired", null);

            // 📤 Do seznamu místností (čekací seznam)
            await _signalRSender.SendToGroupAsync(_listHub, "waitingroom-list", "RoomExpired", roomId);
        }

        public async Task NotifyRoomUpdatedAsync(WaitingRoomDto dto)
        {
            // 👥 Update konkrétní místnosti
            await _signalRSender.SendToGroupAsync(_roomHub, dto.WaitingRoomId.ToString(), "RoomUpdated", dto);

            // 📋 Update seznamu místností
            await _signalRSender.SendToGroupAsync(_listHub, "waitingroom-list", "RoomUpdated", dto);
        }
    }
}
