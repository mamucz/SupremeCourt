using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Sessions;

namespace SupremeCourt.Domain.Interfaces;

public interface IWaitingRoomSessionManager
{
    /// <summary>
    /// Vytvoří novou místnost s daným hráčem jako zakladatelem.
    /// </summary>
    int CreateRoom(IPlayer createdBy);

    /// <summary>
    /// Pokusí se přidat hráče do místnosti. Vrací true, pokud byl přidán.
    /// </summary>
    bool TryJoinPlayer(int roomId, IPlayer player);

    /// <summary>
    /// Pokusí se odebrat hráče z místnosti. Vrací true, pokud byl odstraněn.
    /// </summary>
    bool TryRemovePlayer(int roomId, int playerId);

    /// <summary>
    /// Získá session podle ID místnosti.
    /// </summary>
    WaitingRoomSession? GetSession(int roomId);

    /// <summary>
    /// Odstraní místnost z paměti.
    /// </summary>
    void RemoveSession(int roomId);

    /// <summary>
    /// Vrací všechny aktuální sessiony.
    /// </summary>
    List<WaitingRoomSession> GetAllSessions();
    WaitingRoomSession? GetSessionByPlayerId(int playerId);
}