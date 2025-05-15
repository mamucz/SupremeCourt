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

public class WaitingRoomListService : IWaitingRoomListService
{
    private readonly IWaitingRoomSessionManager _sessionManager;
    private readonly IPlayerRepository _playerRepository;

    public WaitingRoomListService(
        IWaitingRoomSessionManager sessionManager,
        IPlayerRepository playerRepository)
    {
        _sessionManager = sessionManager;
        _playerRepository = playerRepository;
    }

    public async Task<WaitingRoomSession?> CreateWaitingRoomAsync(int createdByPlayerId, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(createdByPlayerId);
        if (player == null)
            return null; // hráč neexistuje

        var roomId = _sessionManager.CreateRoom(player as IPlayer);
        return _sessionManager.GetSession(roomId);
    }

    public async Task<bool> JoinWaitingRoomAsync(Guid waitingRoomId, int playerId, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            return false; // hráč neexistuje

        var result = _sessionManager.TryJoinPlayer(waitingRoomId, player as IPlayer);
        return result;
    }

    public Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync(CancellationToken cancellationToken)
    {
        var sessions = _sessionManager.GetAllSessions();
        var summaries = sessions.Select(WaitingRoomSessionMapper.ToDto).ToList();
        return Task.FromResult(summaries);
    }

    public Task<List<WaitingRoomSession>> GetAllWaitingRoomsAsync(CancellationToken cancellationToken)
    {
        var sessions = _sessionManager.GetAllSessions();
        return Task.FromResult(sessions);
    }
}
