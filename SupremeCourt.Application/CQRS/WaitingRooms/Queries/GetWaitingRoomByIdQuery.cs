using MediatR;
using SupremeCourt.Domain.DTOs;

public class GetWaitingRoomByIdQuery : IRequest<WaitingRoomDto?>
{
    public Guid WaitingRoomId { get; }

    public GetWaitingRoomByIdQuery(Guid waitingRoomId)
    {
        WaitingRoomId = waitingRoomId;
    }
}