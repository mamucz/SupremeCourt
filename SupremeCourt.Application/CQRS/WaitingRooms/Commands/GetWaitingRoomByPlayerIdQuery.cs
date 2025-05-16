using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public record GetWaitingRoomByPlayerIdQuery(int PlayerId) : IRequest<WaitingRoomDto?>;
}
