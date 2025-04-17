using MediatR;
using SupremeCourt.Domain.DTOs;

public class GetWaitingRoomByIdQuery : IRequest<WaitingRoomDto?>
{
    public int WaitingRoomId { get; }

    public GetWaitingRoomByIdQuery(int waitingRoomId)
    {
        WaitingRoomId = waitingRoomId;
    }
}