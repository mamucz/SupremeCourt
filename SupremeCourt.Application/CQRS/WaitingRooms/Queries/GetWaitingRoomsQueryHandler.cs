// SupremeCourt.Application.WaitingRooms.Queries.GetWaitingRoomsQueryHandler.cs

using MediatR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Queries
{
    public class GetWaitingRoomsQueryHandler : IRequestHandler<GetWaitingRoomsQuery, List<WaitingRoomDto>>
    {
        private readonly IWaitingRoomListService _waitingRoomService;

        public GetWaitingRoomsQueryHandler(IWaitingRoomListService waitingRoomService)
        {
            _waitingRoomService = waitingRoomService;
        }

        public async Task<List<WaitingRoomDto>> Handle(GetWaitingRoomsQuery request, CancellationToken cancellationToken)
        {
            return await _waitingRoomService.GetWaitingRoomSummariesAsync();
        }
    }
}