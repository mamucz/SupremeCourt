using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Sessions;
using System.Collections.Concurrent;

public class WaitingRoomSessionManager : IWaitingRoomSessionManager
{
    private readonly ConcurrentDictionary<Guid, WaitingRoomSession> _sessions = new();
    private readonly ILogger<WaitingRoomSessionManager> _logger;
    private readonly IWaitingRoomNotifier _notifier;
    private readonly IWaitingRoomEventHandler _eventHandler;

    public WaitingRoomSessionManager(
        ILogger<WaitingRoomSessionManager> logger,
        IWaitingRoomNotifier notifier,
        IWaitingRoomEventHandler eventHandler)
    {
        _logger = logger;
        _notifier = notifier;
        _eventHandler = eventHandler;
    }

    public WaitingRoomSession? GetSession(Guid roomId)
    {
        _sessions.TryGetValue(roomId, out var session);
        return session;
    }

    public List<WaitingRoomSession> GetAllSessions() => _sessions.Values.ToList();

    public Guid CreateRoom(IPlayer createdBy)
    {
        var session = new WaitingRoomSession(createdBy, RemoveAndNotifyRoomExpired);
        _sessions.TryAdd(session.WaitingRoomId, session);
        _logger.LogInformation("✅ Místnost {RoomId} byla vytvořena.", session.WaitingRoomId);
        return session.WaitingRoomId;
    }

    public bool TryJoinPlayer(Guid roomId, IPlayer player)
    {
        if (!_sessions.TryGetValue(roomId, out var session))
            return false;

        return session.TryAddPlayer(player);
    }

    public bool TryRemovePlayer(Guid roomId, int playerId)
    {
        if (_sessions.TryGetValue(roomId, out var session))
        {
            return session.TryRemovePlayer(playerId);
        }
        return false;
    }

    public void RemoveSession(Guid roomId)
    {
        if (_sessions.TryRemove(roomId, out var session))
        {
            session.Dispose();
            _logger.LogInformation("🗑️ Místnost {RoomId} byla odstraněna.", roomId);
        }
    }

    private async void RemoveAndNotifyRoomExpired(Guid roomId)
    {
        RemoveSession(roomId);
        await _eventHandler.HandleRoomExpiredAsync(roomId);
    }

    public WaitingRoomSession? GetSessionByPlayerId(int playerId)
    {
        return _sessions.Values.FirstOrDefault(s => s.Players.Any(p => p.Id == playerId));
    }
}
