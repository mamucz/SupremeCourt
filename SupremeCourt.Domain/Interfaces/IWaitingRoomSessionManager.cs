using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Sessions;

namespace SupremeCourt.Domain.Interfaces;

public interface IWaitingRoomSessionManager
{
    /// <summary>
    /// Vytvoří novou místnost s daným hráčem jako zakladatelem.
    /// </summary>
    Guid CreateRoom(IPlayer createdBy);

    /// <summary>
    /// Pokusí se přidat hráče do místnosti. Vrací true, pokud byl přidán.
    /// </summary>
    bool TryJoinPlayer(Guid roomId, IPlayer player);

    /// <summary>
    /// Pokusí se odebrat hráče z místnosti. Vrací true, pokud byl odstraněn.
    /// </summary>
    bool TryRemovePlayer(Guid roomId, Guid playerId);

    /// <summary>
    /// Získá session podle ID místnosti.
    /// </summary>
    WaitingRoomSession? GetSession(Guid roomId);

    /// <summary>
    /// Odstraní místnost z paměti.
    /// </summary>
    void RemoveSession(Guid roomId);

    /// <summary>
    /// Vrací všechny aktuální sessiony.
    /// </summary>
    List<WaitingRoomSession> GetAllSessions();
    WaitingRoomSession? GetSessionByPlayerId(Guid playerId);
    void RegisterCallbacks(Func<Guid, int, Task> onTick, Func<Guid, Task> onExpired);
    Task<bool> AddAiPlayerAsync(Guid roomId, string aiType, CancellationToken ct);
    bool AddPlayerToRoomAsync(Guid roomId, IPlayer player);
}