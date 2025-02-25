using Microsoft.AspNetCore.SignalR;

namespace SupremeCourt.Infrastructure.SignalR
{
    public class GameHub : Hub
    {
        public async Task SendGameUpdate(string gameId, string message)
        {
            await Clients.Group(gameId).SendAsync("GameUpdated", message);
        }

        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }
    }
}
