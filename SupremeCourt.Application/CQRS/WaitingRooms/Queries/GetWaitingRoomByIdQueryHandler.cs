using MediatR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Queries;

public class GetWaitingRoomByIdQueryHandler : IRequestHandler<GetWaitingRoomByIdQuery, WaitingRoomDto?>
{
    private readonly IWaitingRoomSessionManager _sessionManager;

    public GetWaitingRoomByIdQueryHandler(IWaitingRoomSessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public Task<WaitingRoomDto?> Handle(GetWaitingRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var session = _sessionManager.GetSession(request.WaitingRoomId);
        if (session == null)
            return Task.FromResult<WaitingRoomDto?>(null);

        var dto = new WaitingRoomDto
        {
            WaitingRoomId = session.WaitingRoomId,
            CreatedAt = session.CreatedAt,
            CreatedByPlayerId = session.CreatedBy.Id,
            Players = session.Players.Select(p => new PlayerDto
            {
                PlayerId =  p.Id,
                Username = p.Username,
            }).ToList(),
            TimeLeftSeconds = session.GetTimeLeft(),
        };

        return Task.FromResult(dto);
    }
}