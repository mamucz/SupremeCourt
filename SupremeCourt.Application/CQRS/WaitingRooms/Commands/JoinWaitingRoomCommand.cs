using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public class JoinWaitingRoomCommand : IRequest<bool>
    {
        public Guid WaitingRoomId { get; set; }
        public int PlayerId { get; set; }

        public JoinWaitingRoomCommand(Guid waitingRoomId, int playerId)
        {
            WaitingRoomId = waitingRoomId;
            PlayerId = playerId;
        }
    }
}
