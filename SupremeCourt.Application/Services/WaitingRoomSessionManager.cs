using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Sessions;
using System.Collections.Concurrent;

public class WaitingRoomSessionManager : IWaitingRoomSessionManager
{
    private readonly ConcurrentDictionary<Guid, WaitingRoomSession> _sessions = new();
    private readonly ILogger<WaitingRoomSessionManager> _logger;
    private readonly IWaitingRoomNotifier _notifier;
    private readonly int _expirationSeconds;
    private readonly IServiceScopeFactory _scopeFactory; // ⬅️ místo IAiPlayerFactory

    private Func<Guid, int, Task>? _onTickCallback;
    private Func<Guid, Task>? _onExpiredCallback;

    public WaitingRoomSessionManager(
        ILogger<WaitingRoomSessionManager> logger,
        IWaitingRoomNotifier notifier,
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory) // ⬅️ změna
    {
        _logger = logger;
        _notifier = notifier;
        _scopeFactory = scopeFactory;

        // default např. 15 min (900 s); když klíč chybí nebo <= 0, padne to na 60 s
        _expirationSeconds = configuration.GetValue<int?>("WaitingRoom:ExpirationMinutes") is int minutes && minutes > 0
            ? minutes * 60
            : 60;
    }

    public WaitingRoomSession? GetSession(Guid roomId)
    {
        _sessions.TryGetValue(roomId, out var session);
        return session;
    }

    public List<WaitingRoomSession> GetAllSessions() => _sessions.Values.ToList();

    public Guid CreateRoom(IPlayer createdBy)
    {
        // Preferuj, ať WaitingRoomSession přijímá async callback (Func<Guid, Task>)
        var session = new WaitingRoomSession(createdBy, RemoveAndNotifyRoomExpiredAsync, _expirationSeconds);

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

    // ⬇️ změněno na Task (ne async void)
    private async Task RemoveAndNotifyRoomExpiredAsync(Guid roomId)
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

    // --- PŘÍKLAD: když někde uvnitř potřebuješ IAiPlayerFactory / IPlayerRepository ---
    public async Task<bool> AddAiPlayerAsync(Guid roomId, string aiType, CancellationToken ct)
    {
        if (!_sessions.TryGetValue(roomId, out var session))
            return false;

        using var scope = _scopeFactory.CreateScope();
        var aiFactory = scope.ServiceProvider.GetRequiredService<IAiPlayerFactory>();
        var playerRepo = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();

        // zajisti DB entitu pro AI hráče daného typu
        await playerRepo.EnsureAiPlayerExistsAsync(aiType, ct);

        // vytvoř runtime AI hráče (IAiPlayer by měl implementovat IPlayer nebo mít adaptér)
        var aiPlayer = await aiFactory.CreateAsync(aiType);

        return session.TryAddPlayer(aiPlayer);
    }
}
