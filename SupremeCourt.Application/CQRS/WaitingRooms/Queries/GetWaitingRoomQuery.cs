using MediatR;
using SupremeCourt.Domain.DTOs;


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
