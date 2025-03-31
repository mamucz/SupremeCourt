using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SupremeCourt.Application.Sessions;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Logic;
using SupremeCourt.Domain.Mappings;
using SupremeCourt.Domain.Sessions;

namespace SupremeCourt.Application.Services
{
    public class WaitingRoomListService : IWaitingRoomListService
    {
        private readonly IWaitingRoomRepository _waitingRoomRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<WaitingRoomListService> _logger;
        private readonly IWaitingRoomListNotifier _waitingRoomListNotifier; // ✅ Použití nového notifieru
        private readonly IWaitingRoomNotifier _waitingRoomNotifier;
        private readonly WaitingRoomSessionManager _sessionManager;
        private readonly WaitingRoomMapper _waitingRoomMapper;

        public WaitingRoomListService(
            IWaitingRoomRepository waitingRoomRepository,
            IGameRepository gameRepository,
            IPlayerRepository playerRepository,
            IGameService gameService,
            IWaitingRoomListNotifier waitingRoomListNotifier,
            IWaitingRoomNotifier waitingRoomNotifier,
            WaitingRoomSessionManager sessionManager,
            ILogger<WaitingRoomListService> logger)
        {
            _waitingRoomRepository = waitingRoomRepository;
            _playerRepository = playerRepository;
            _waitingRoomListNotifier = waitingRoomListNotifier;
            _waitingRoomNotifier = waitingRoomNotifier;
            _sessionManager = sessionManager;
            _logger = logger;
            _waitingRoomMapper = new WaitingRoomMapper();
        }

        public async Task<WaitingRoom?> CreateWaitingRoomAsync(int createdByPlayerId)
        {
            var player = await _playerRepository.GetByIdAsync(createdByPlayerId);
            if (player == null) return null;

            var waitingRoom = new WaitingRoom
            {
                CreatedByPlayerId = createdByPlayerId,
                Players = new List<Player> ()
            };

            await _waitingRoomRepository.AddAsync(waitingRoom);

            // ✅ Po uložení vytvoř runtime session a přidej ji do manageru
            var session = _waitingRoomMapper.ToSession(waitingRoom);
            _sessionManager.AddSession(session);

            // Po vytvoření místnosti odešli notifikaci
            await _waitingRoomListNotifier.NotifyWaitingRoomCreatedAsync(new
            {
                WaitingRoomId = waitingRoom.Id,
                CreatedAt = waitingRoom.CreatedAt,
                CreatedBy = player.User?.Username ?? "Neznámý", 
                PlayerCount = 0
            });
            return waitingRoom;
        }
        public async Task<bool> JoinWaitingRoomAsync(int waitingRoomId, int playerId)
        {
            // ⛔ Zjisti, zda už není v jiné místnosti
            var existingRoom = await _waitingRoomRepository.GetRoomByPlayerIdAsync(playerId);
            if (existingRoom != null)
            {
                _logger.LogWarning("Hráč {PlayerId} je již ve waiting room #{RoomId}", playerId, existingRoom.Id);
                return false;
            }

            var waitingRoom = await _waitingRoomRepository.GetByIdAsync(waitingRoomId);
            if (waitingRoom == null) return false;

            if (waitingRoom.Players.Count >= GameRules.MaxPlayers)
            {
                _logger.LogWarning($"Hráč {playerId} se pokusil připojit do plné místnosti {waitingRoomId}.");
                return false;
            }

            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null) return false;

            waitingRoom.Players.Add(player);
            await _waitingRoomRepository.UpdateAsync(waitingRoom);

            await _waitingRoomNotifier.NotifyPlayerJoinedAsync(waitingRoomId, player.User.Username);

            return true;
        }


        public async Task<List<WaitingRoom>> GetAllWaitingRoomsAsync() // ✅ Přidáno
        {
            return await _waitingRoomRepository.GetAllAsync();
        }

        public async Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync()
        {
            var all = await _waitingRoomRepository.GetAllAsync();

            var result = new List<WaitingRoomDto>();

            foreach (var wr in all.Where(wr => wr.Players.Count < GameRules.MaxPlayers))
            {
                var creator = await _playerRepository.GetByIdAsync(wr.CreatedByPlayerId);
                var creatorName = creator?.User?.Username ?? "Neznámý";
                var creatorUserId = creator?.User?.Id ?? -1;

                result.Add(new WaitingRoomDto
                {
                    WaitingRoomId = wr.Id,
                    CreatedAt = wr.CreatedAt,
                    CreatedByPlayerId = creatorUserId,
                });
            }

            return result;
        }
    }
}
