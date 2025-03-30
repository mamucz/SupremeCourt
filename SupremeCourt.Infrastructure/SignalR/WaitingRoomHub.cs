using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SupremeCourt.Infrastructure.SignalR
{
    [Authorize]
    public class WaitingRoomHub : Hub
    {
        /// <summary>
        /// Připojí klienta do skupiny pro konkrétní místnost (podle ID).
        /// </summary>
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }

        /// <summary>
        /// Odpojí klienta ze skupiny při opuštění místnosti.
        /// </summary>
        public async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }
    }
}