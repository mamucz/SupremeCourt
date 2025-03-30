using MediatR;
using SupremeCourt.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Queries
{
    public class GetWaitingRoomQuery : IRequest<WaitingRoomDto>
    {
        public int WaitingRoomId { get; set; }

        public GetWaitingRoomQuery(int id)
        {
            WaitingRoomId = id;
        }
    }
}
