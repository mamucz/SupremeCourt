using MediatR;
using SupremeCourt.Application.CQRS.WaitingRooms.Commands;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Mappings;

public class LeaveWaitingRoomCommandHandler : IRequestHandler<LeaveWaitingRoomCommand, bool>
{
    private readonly IWaitingRoomSessionManager _sessionManager;
    private readonly IWaitingRoomEventHandler _eventHandler;

    public LeaveWaitingRoomCommandHandler(
        IWaitingRoomSessionManager sessionManager,
        IWaitingRoomEventHandler eventHandler)
    {
        _sessionManager = sessionManager;
        _eventHandler = eventHandler;
    }

    public async Task<bool> Handle(LeaveWaitingRoomCommand request, CancellationToken cancellationToken)
    {
        var session = _sessionManager.GetSession(request.WaitingRoomId);
        if (session == null)
            return false;

        var removed = session.TryRemovePlayer(request.PlayerId);
        if (!removed)
            return false;

        var dto = WaitingRoomSessionMapper.ToDto(session);
        await _eventHandler.NotifyRoomUpdatedAsync(request.WaitingRoomId, cancellationToken);
        return true;
    }
}