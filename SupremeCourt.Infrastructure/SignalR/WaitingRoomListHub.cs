using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using SupremeCourt.Infrastructure.Interfaces;

namespace SupremeCourt.Infrastructure.SignalR
{
    public class WaitingRoomListHub : Hub
    {
        private static readonly ConcurrentDictionary<string, byte> _connectedUsers = new();
        private readonly ISignalRSender _signalRSender;
        private readonly IHubContext<WaitingRoomListHub> _hubContext;

        public WaitingRoomListHub(ISignalRSender signalRSender, IHubContext<WaitingRoomListHub> hubContext)
        {
            _signalRSender = signalRSender;
            _hubContext = hubContext;
        }

        public async Task JoinRoom(string gameId)
        {
            var connectionId = Context.ConnectionId;

            // Použijeme thread-safe kolekci
            if (_connectedUsers.TryAdd(connectionId, 0))
            {
                await Groups.AddToGroupAsync(connectionId, gameId);

                // Odeslání zprávy jednotnou metodou
                await _signalRSender.SendToGroupAsync(_hubContext, gameId, "PlayerJoined", $"Hráč se připojil k místnosti {gameId}");
            }
        }

        public async Task JoinWaitingRoomList()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "waitingroom-list");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connectedUsers.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }
    }
}