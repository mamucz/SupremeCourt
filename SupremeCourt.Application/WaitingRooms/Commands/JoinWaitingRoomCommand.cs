using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Application.WaitingRooms.Commands
{
    public class JoinWaitingRoomCommand : IRequest<bool>
    {
        public int WaitingRoomId { get; set; }
        public int PlayerId { get; set; }

        public JoinWaitingRoomCommand(int waitingRoomId, int playerId)
        {
            WaitingRoomId = waitingRoomId;
            PlayerId = playerId;
        }
    }
}
