using MediatR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Queries;

public class GetWaitingRoomQueryHandler : IRequestHandler<GetWaitingRoomQuery, WaitingRoomDto?>
{
    private readonly IWaitingRoomSessionManager _sessionManager;

    public GetWaitingRoomQueryHandler(IWaitingRoomSessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public Task<WaitingRoomDto?> Handle(GetWaitingRoomQuery request, CancellationToken cancellationToken)
    {
        var session = _sessionManager.GetSession(request.WaitingRoomId);
        if (session == null)
            return Task.FromResult<WaitingRoomDto?>(null);

        var dto = new WaitingRoomDto
        {
            WaitingRoomId = session.WaitingRoomId,
            CreatedAt = session.CreatedAt,
            CreatedByPlayerId = session.CreatedBy.Id,
            CreatedByPlayerName = session.CreatedBy.Username,
            TimeLeftSeconds = session.GetTimeLeft(),
            Players = session.Players.Select(p => new PlayerDto
            {
                PlayerId = p.Id,
                Username = p.Username,
            }).ToList()
        };

        return Task.FromResult(dto);
    }
}