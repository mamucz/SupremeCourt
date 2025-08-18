using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Sessions;
using System.Collections.Concurrent;

public class WaitingRoomSessionManager : IWaitingRoomSessionManager
{
    private readonly ConcurrentDictionary<Guid, WaitingRoomSession> _sessions = new();
    private readonly ILogger<WaitingRoomSessionManager> _logger;
    private readonly IWaitingRoomNotifier _notifier;
    private readonly int _expirationSeconds;
    private readonly IAIPlayerFactory _aiFactory;

    private Func<Guid, int, Task>? _onTickCallback;
    private Func<Guid, Task>? _onExpiredCallback;

    public WaitingRoomSessionManager(
        ILogger<WaitingRoomSessionManager> logger,
        IWaitingRoomNotifier notifier,
        IConfiguration configuration,
        IAIPlayerFactory aiFactory)
    {
        _logger = logger;
        _notifier = notifier;

        // 🕒 Načtení hodnoty z konfigurace, default 900 sekund (15 minut), pokud nenalezena
        _expirationSeconds = configuration.GetValue<int?>("WaitingRoom:ExpirationMinutes") is int minutes && minutes > 0
            ? minutes * 60
            : 60; // fallback na 60 sekund
        _aiFactory = aiFactory;
    }

    public WaitingRoomSession? GetSession(Guid roomId)
    {
        _sessions.TryGetValue(roomId, out var session);
        return session;
    }

    public List<WaitingRoomSession> GetAllSessions() => _sessions.Values.ToList();

    public Guid CreateRoom(IPlayer createdBy)
    {
        var session = new WaitingRoomSession(createdBy, RemoveAndNotifyRoomExpired, _expirationSeconds, _aiFactory);

        AttachCallbacks(session);

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

    public WaitingRoomSession? GetSessionByPlayerId(int playerId)
    {
        return _sessions.Values.FirstOrDefault(s => s.Players.Any(p => p.Id == playerId));
    }

    private void AttachCallbacks(WaitingRoomSession session)
    {
        if (_onTickCallback != null)
            session.OnCountdownTick += _onTickCallback;

        if (_onExpiredCallback != null)
            session.OnRoomExpired += _onExpiredCallback;
    }

    private async void RemoveAndNotifyRoomExpired(Guid roomId)
    {
        RemoveSession(roomId);
        if (_onExpiredCallback != null)
        {
            await _onExpiredCallback(roomId);
        }
    }

    public void RegisterCallbacks(Func<Guid, int, Task> onTick, Func<Guid, Task> onExpired)
    {
        _onTickCallback = onTick;
        _onExpiredCallback = onExpired;

        foreach (var session in _sessions.Values)
        {
            AttachCallbacks(session);
        }
    }
}
