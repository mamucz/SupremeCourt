using MediatR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Queries;

public class GetWaitingRoomsQueryHandler : IRequestHandler<GetWaitingRoomsQuery, List<WaitingRoomDto>>
{
    private readonly IWaitingRoomSessionManager _sessionManager;

    public GetWaitingRoomsQueryHandler(IWaitingRoomSessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public Task<List<WaitingRoomDto>> Handle(GetWaitingRoomsQuery request, CancellationToken cancellationToken)
    {
        var sessions = _sessionManager.GetAllSessions();

        var result = sessions.Select(session => new WaitingRoomDto
        {
            WaitingRoomId = session.WaitingRoomId,
            CreatedAt = session.CreatedAt,
            CreatedByPlayerId = session.CreatedBy.Id,
            Players = session.Players.Select(p => new PlayerDto
            {
                PlayerId = p.Id,
                Username = p.Username,
            }).ToList(),
            TimeLeftSeconds = session.GetTimeLeft(),
        }).ToList();

        return Task.FromResult(result);
    }
}