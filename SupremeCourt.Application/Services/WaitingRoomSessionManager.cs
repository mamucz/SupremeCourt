using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Sessions;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Mappings;
using MediatR;
using SupremeCourt.Application.CQRS.WaitingRooms.Commands;

/// <summary>
/// Správce běžících (runtime) čekacích místností. 
/// Uchovává seznam aktivních místností, umožňuje jejich vytvoření, správu hráčů a signalizaci událostí.
/// 
/// Každá místnost existuje pouze v paměti po dobu její životnosti.
/// </summary>
public class WaitingRoomSessionManager : IWaitingRoomSessionManager
{
    private readonly ConcurrentDictionary<Guid, WaitingRoomSession> _sessions = new();
    private readonly ILogger<WaitingRoomSessionManager> _logger;
    private readonly IWaitingRoomNotifier _notifier;
    private readonly int _expirationSeconds;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;

    private Func<Guid, int, Task>? _onTickCallback;
    private Func<Guid, Task>? _onExpiredCallback;

    /// <summary>
    /// Inicializuje novou instanci správce čekacích místností.
    /// </summary>
    /// <param name="logger">Logger pro logování událostí.</param>
    /// <param name="notifier">Komponenta pro odesílání SignalR notifikací.</param>
    /// <param name="configuration">Konfigurace pro dobu životnosti místnosti.</param>
    /// <param name="scopeFactory">Továrna pro vytváření DI scope (např. pro přístup k DB).</param>
    public WaitingRoomSessionManager(
        ILogger<WaitingRoomSessionManager> logger,
        IWaitingRoomNotifier notifier,
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        IUserRepository userRepository,
        IMediator mediator)
    {
        _logger = logger;
        _notifier = notifier;
        _scopeFactory = scopeFactory;

        _expirationSeconds = configuration.GetValue<int?>("WaitingRoom:ExpirationMinutes") is int minutes && minutes > 0
            ? minutes * 60
            : 60;
        _userRepository = userRepository;
        _mediator = mediator;
    }

    /// <summary>
    /// Vrátí konkrétní běžící místnost podle ID.
    /// </summary>
    public WaitingRoomSession? GetSession(Guid roomId)
    {
        _sessions.TryGetValue(roomId, out var session);
        return session;
    }

    /// <summary>
    /// Vrátí všechny aktuálně běžící čekací místnosti.
    /// </summary>
    public List<WaitingRoomSession> GetAllSessions() => _sessions.Values.ToList();

    /// <summary>
    /// Vytvoří novou čekací místnost pro zadaného hráče.
    /// </summary>
    /// <param name="createdBy">Hráč, který místnost vytváří.</param>
    /// <returns>ID nově vytvořené místnosti.</returns>
    public Guid CreateRoom(IPlayer createdBy)
    {
        var session = new WaitingRoomSession(createdBy, RemoveAndNotifyRoomExpiredAsync, _expirationSeconds);
        AttachCallbacks(session);
        _sessions.TryAdd(session.WaitingRoomId, session);
        _logger.LogInformation("✅ Místnost {RoomId} byla vytvořena.", session.WaitingRoomId);
        return session.WaitingRoomId;
    }

    /// <summary>
    /// Pokusí se připojit hráče do existující místnosti.
    /// </summary>
    public bool TryJoinPlayer(Guid roomId, IPlayer player)
    {
        if (!_sessions.TryGetValue(roomId, out var session))
            return false;

        return session.TryAddPlayer(player);
    }

    /// <summary>
    /// Pokusí se odstranit hráče z dané místnosti.
    /// </summary>
    public bool TryRemovePlayer(Guid roomId, Guid playerId)
    {
        if (_sessions.TryGetValue(roomId, out var session))
        {
            return session.TryRemovePlayer(playerId);
        }
        return false;
    }

    /// <summary>
    /// Trvale odstraní celou místnost.
    /// </summary>
    public void RemoveSession(Guid roomId)
    {
        if (_sessions.TryRemove(roomId, out var session))
        {
            session.Dispose();
            _logger.LogInformation("🗑️ Místnost {RoomId} byla odstraněna.", roomId);
        }
    }

    /// <summary>
    /// Vrátí místnost, ve které se nachází hráč podle jeho ID.
    /// </summary>
    public WaitingRoomSession? GetSessionByPlayerId(Guid playerId)
    {
        return _sessions.Values.FirstOrDefault(s => s.Players.Any(p => p.Id == playerId));
    }

    /// <summary>
    /// Přiřadí událostem místnosti vlastní callbacky.
    /// </summary>
    private void AttachCallbacks(WaitingRoomSession session)
    {
        if (_onTickCallback != null)
            session.OnCountdownTick += _onTickCallback;

        if (_onExpiredCallback != null)
            session.OnRoomExpired += _onExpiredCallback;
    }

    /// <summary>
    /// Odstraní místnost a zároveň zavolá externí callback pro zpracování expirace.
    /// </summary>
    private async Task RemoveAndNotifyRoomExpiredAsync(Guid roomId)
    {
        RemoveSession(roomId);
        if (_onExpiredCallback != null)
        {
            await _onExpiredCallback(roomId);
        }
    }

    /// <summary>
    /// Zaregistruje callbacky, které se mají volat při každém ticku odpočtu a při expiraci místnosti.
    /// </summary>
    public void RegisterCallbacks(Func<Guid, int, Task> onTick, Func<Guid, Task> onExpired)
    {
        _onTickCallback = onTick;
        _onExpiredCallback = onExpired;

        foreach (var session in _sessions.Values)
        {
            AttachCallbacks(session);
        }
    }

    /// <summary>
    /// Přidá do místnosti nového AI hráče daného typu.
    /// Využívá DI scope pro získání <see cref="IAiPlayerFactory"/> a <see cref="IPlayerRepository"/>.
    /// </summary>
    /// <param name="roomId">ID místnosti.</param>
    /// <param name="aiType">Typ AI hráče (např. "Random").</param>
    /// <param name="ct">Storno token.</param>
    /// <returns>True, pokud byl hráč úspěšně přidán.</returns>
    public async Task<bool> AddAiPlayerAsync(Guid roomId, string aiType, CancellationToken ct)
    {
        var aiUser = await _userRepository.GetAiUserByTypeAsync(aiType);
        Player aiPlayer = new Player(aiUser);
        var result = await _mediator.Send(new JoinWaitingRoomCommand(roomId, aiPlayer));
        if (!result)
            _logger.LogError("Can't add AI Player");
        return result;
    }

    public object TypeNameAssemblyFormatHandling { get; set; }

    public async Task<bool> TryAddPlayerToRoomAsync(Guid roomId, IPlayer player, CancellationToken ct = default)
    {
        var session = GetSession(roomId);
        if (session == null)
        {
            _logger.LogWarning("❌ Místnost {RoomId} neexistuje.", roomId);
            return false;
        }

        if (session.IsFull)
        {
            _logger.LogWarning("⚠️ Místnost {RoomId} je plná. Hráč {PlayerId} nemůže vstoupit.", roomId, player.Id);
            return false;
        }

        if (session.Players.Any(p => p.Id == player.Id) && player.IsAi == false)
        {
            _logger.LogInformation("ℹ️ Hráč {PlayerId} je již připojen do místnosti {RoomId}.", player.Id, roomId);
            return true;
        }

        var success = session.TryAddPlayer(player);
        if (!success)
        {
            _logger.LogWarning("❌ Nepodařilo se přidat hráče {PlayerId} do místnosti {RoomId}.", player.Id, roomId);
            return false;
        }

        _logger.LogInformation("✅ Hráč {PlayerId} byl přidán do místnosti {RoomId}.", player.Id, roomId);

        // mapování hráče na DTO (ručně, protože může jít o AI hráče nebo běžného hráče)
        var dto = new PlayerDto
        {
            PlayerId = player.Id,
            Username = player.Username,
        };

        await _notifier.NotifyPlayerJoinedAsync(roomId, dto);

        return true;
    }

    public bool AddPlayerToRoomAsync(Guid roomId, IPlayer player)
    {
        throw new NotImplementedException();
    }
}
