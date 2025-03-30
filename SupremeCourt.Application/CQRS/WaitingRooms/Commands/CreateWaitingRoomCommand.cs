using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
        public class CreateWaitingRoomCommand : IRequest<WaitingRoomCreatedDto>
        {
            public int PlayerId { get; set; }

            public CreateWaitingRoomCommand(int playerId)
            {
                PlayerId = playerId;
            }
        }
}
