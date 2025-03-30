using MediatR;
using SupremeCourt.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Queries
{
    public class GetWaitingRoomsQuery : IRequest<List<WaitingRoomDto>>
    {
    }
}
