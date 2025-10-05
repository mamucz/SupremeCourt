using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public class JoinWaitingRoomCommand : IRequest<bool>
    {
        public Guid WaitingRoomId { get; set; }
        public Player Player { get; set; }

        public JoinWaitingRoomCommand(Guid waitingRoomId, Player player)
        {
            WaitingRoomId = waitingRoomId;
            this.Player = player;
        }
    }
}
