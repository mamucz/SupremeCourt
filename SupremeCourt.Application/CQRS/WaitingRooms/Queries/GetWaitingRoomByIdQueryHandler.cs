using MediatR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Mappings; // ⬅️ Přidat using

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

        var dto = WaitingRoomSessionMapper.ToDto(session);

        return Task.FromResult(dto);
    }
}