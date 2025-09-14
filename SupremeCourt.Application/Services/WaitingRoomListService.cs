using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Mappings;
using SupremeCourt.Domain.Sessions;

namespace SupremeCourt.Application.Services;

/// <summary>
/// Služba pro správu seznamu čekacích místností (Waiting Rooms).
/// Poskytuje operace pro vytvoření nové místnosti, připojení hráče do existující místnosti,
/// a získání přehledu všech aktivních místností.
/// 
/// Tato služba slouží jako aplikační vrstva nad <see cref="IWaitingRoomSessionManager"/> a <see cref="IPlayerRepository"/>.
/// </summary>
public class WaitingRoomListService : IWaitingRoomListService
{
    private readonly IWaitingRoomSessionManager _sessionManager;
    private readonly IPlayerRepository _playerRepository;

    /// <summary>
    /// Inicializuje novou instanci <see cref="WaitingRoomListService"/>.
    /// </summary>
    /// <param name="sessionManager">Správce runtime čekacích místností.</param>
    /// <param name="playerRepository">Repozitář hráčů pro načítání informací z databáze.</param>
    public WaitingRoomListService(
        IWaitingRoomSessionManager sessionManager,
        IPlayerRepository playerRepository)
    {
        _sessionManager = sessionManager;
        _playerRepository = playerRepository;
    }

    /// <summary>
    /// Vytvoří novou čekací místnost pro hráče se zadaným ID.
    /// </summary>
    /// <param name="createdByPlayerId">ID hráče, který místnost zakládá.</param>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns>Instance <see cref="WaitingRoomSession"/> nebo <c>null</c>, pokud hráč neexistuje.</returns>
    public async Task<WaitingRoomSession?> CreateWaitingRoomAsync(int createdByPlayerId, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(createdByPlayerId);
        if (player == null)
            return null; // hráč neexistuje

        var roomId = _sessionManager.CreateRoom(player as IPlayer);
        return _sessionManager.GetSession(roomId);
    }

    /// <summary>
    /// Připojí hráče do existující čekací místnosti.
    /// </summary>
    /// <param name="waitingRoomId">ID místnosti.</param>
    /// <param name="playerId">ID hráče.</param>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns><c>true</c>, pokud byl hráč úspěšně připojen; jinak <c>false</c>.</returns>
    public async Task<bool> JoinWaitingRoomAsync(Guid waitingRoomId, int playerId, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            return false; // hráč neexistuje

        var result = _sessionManager.TryJoinPlayer(waitingRoomId, player as IPlayer);
        return result;
    }

    /// <summary>
    /// Vrátí seznam přehledů všech aktivních čekacích místností.
    /// </summary>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns>Seznam DTO s informacemi o čekacích místnostech.</returns>
    public Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync(CancellationToken cancellationToken)
    {
        var sessions = _sessionManager.GetAllSessions();
        var summaries = sessions.Select(WaitingRoomSessionMapper.ToDto).ToList();
        return Task.FromResult(summaries);
    }

    /// <summary>
    /// Vrátí všechny aktivní instance čekacích místností včetně detailů.
    /// </summary>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
    /// <returns>Seznam aktivních <see cref="WaitingRoomSession"/>.</returns>
    public Task<List<WaitingRoomSession>> GetAllWaitingRoomsAsync(CancellationToken cancellationToken)
    {
        var sessions = _sessionManager.GetAllSessions();
        return Task.FromResult(sessions);
    }
}
