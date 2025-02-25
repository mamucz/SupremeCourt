using Microsoft.AspNetCore.SignalR;

namespace SupremeCourt.Infrastructure.SignalR
{
    public class WaitingRoomHub : Hub
    {
        public async Task JoinRoom(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Group(gameId).SendAsync("PlayerJoined", $"Hráč se připojil k místnosti {gameId}");
        }
    }
}