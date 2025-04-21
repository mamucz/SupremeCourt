using Microsoft.AspNetCore.SignalR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Infrastructure.Interfaces;
using SupremeCourt.Infrastructure.SignalR;

namespace SupremeCourt.Infrastructure.Services
{
    public class WaitingRoomListNotifier : IWaitingRoomListNotifier
    {
        private readonly ISignalRSender _signalRSender;
        private readonly IHubContext<WaitingRoomListHub> _hubContext;

        public WaitingRoomListNotifier(
            ISignalRSender signalRSender,
            IHubContext<WaitingRoomListHub> hubContext)
        {
            _signalRSender = signalRSender;
            _hubContext = hubContext;
        }

        public Task NotifyPlayerJoinedAsync(int gameId, string playerName)
        {
            return _signalRSender.SendToGroupAsync(_hubContext, gameId.ToString(), "PlayerJoined", playerName);
        }

        public Task NotifyWaitingRoomCreatedAsync(WaitingRoomDto roomDto)
        {
            return _signalRSender.SendToGroupAsync(_hubContext, "waitingroom-list", "NewWaitingRoomCreated", roomDto);
        }
    }
}