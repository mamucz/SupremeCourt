using Microsoft.AspNetCore.SignalR;
using SupremeCourt.Infrastructure.Interfaces;
using SupremeCourt.Infrastructure.SignalR;

namespace SupremeCourt.Infrastructure.SignalR
{
    public class GameHub : Hub
    {
        private readonly ISignalRSender _signalRSender;
        private readonly IHubContext<GameHub> _hubContext;

        public GameHub(ISignalRSender signalRSender, IHubContext<GameHub> hubContext)
        {
            _signalRSender = signalRSender;
            _hubContext = hubContext;
        }

        public async Task SendGameUpdate(string gameId, string message)
        {
            await _signalRSender.SendToGroupAsync(_hubContext, gameId, "GameUpdated", message);
        }

        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }
    }
}