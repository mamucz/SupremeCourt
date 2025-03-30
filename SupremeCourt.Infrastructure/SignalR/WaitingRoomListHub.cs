using Microsoft.AspNetCore.SignalR;

namespace SupremeCourt.Infrastructure.SignalR
{
    public class WaitingRoomListHub : Hub
    {
        private static readonly HashSet<string> _connectedUsers = new();
        public async Task JoinRoom(string gameId)
        {
            var connectionId = Context.ConnectionId;

            // Kontrola, zda už je uživatel připojen
            if (!_connectedUsers.Contains(connectionId))
            {
                _connectedUsers.Add(connectionId);
                await Groups.AddToGroupAsync(connectionId, gameId);
                await Clients.Group(gameId).SendAsync("PlayerJoined", $"Hráč se připojil k místnosti {gameId}");
            }
        }
        public async Task JoinWaitingRoomList()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "waitingroom-list");
        }
    }
}