using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using SupremeCourt.Infrastructure.Interfaces;

namespace SupremeCourt.Infrastructure.SignalR
{
    [Authorize]
    public class WaitingRoomHub : Hub
    {
        private readonly ISignalRSender _signalRSender;

        [Obsolete("Use ISignalRSender instead.")]
        private readonly IHubContext<WaitingRoomHub> _hubContext;

        public WaitingRoomHub(ISignalRSender signalRSender, IHubContext<WaitingRoomHub> hubContext)
        {
            _signalRSender = signalRSender;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Připojí klienta do skupiny pro konkrétní místnost (podle ID).
        /// </summary>
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            // Případně můžeš informovat skupinu:
            // await _signalRSender.SendToGroupAsync(_hubContext, roomId, "PlayerJoined", Context.UserIdentifier);
        }

        /// <summary>
        /// Odpojí klienta ze skupiny při opuštění místnosti.
        /// </summary>
        public async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            // await _signalRSender.SendToGroupAsync(_hubContext, roomId, "PlayerLeft", Context.UserIdentifier);
        }
    }
}