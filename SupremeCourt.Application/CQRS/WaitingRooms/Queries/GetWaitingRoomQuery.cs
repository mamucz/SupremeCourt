using MediatR;
using SupremeCourt.Domain.DTOs;


namespace SupremeCourt.Application.CQRS.WaitingRooms.Queries
{
    public class GetWaitingRoomQuery : IRequest<WaitingRoomDto>
    {
        public Guid WaitingRoomId { get; set; }

        public GetWaitingRoomQuery(Guid id)
        {
            WaitingRoomId = id;
        }
    }
}
