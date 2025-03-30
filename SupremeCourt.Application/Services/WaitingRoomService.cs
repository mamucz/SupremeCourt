using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Logic;

namespace SupremeCourt.Application.Services
{
    public class WaitingRoomService : IWaitingRoomService
    {
        private readonly IWaitingRoomRepository _waitingRoomRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<WaitingRoomService> _logger;
        private readonly IGameService _gameService;
        private readonly IWaitingRoomNotifier _waitingRoomNotifier; // ✅ Použití nového notifieru

        public WaitingRoomService(
            IWaitingRoomRepository waitingRoomRepository,
            IGameRepository gameRepository,
            IPlayerRepository playerRepository,
            IGameService gameService,
            IWaitingRoomNotifier waitingRoomNotifier,
            ILogger<WaitingRoomService> logger)
        {
            _waitingRoomRepository = waitingRoomRepository;
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
            _gameService = gameService;
            _waitingRoomNotifier = waitingRoomNotifier;
            _logger = logger;
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
            // Po vytvoření místnosti odešli notifikaci
            await _waitingRoomNotifier.NotifyWaitingRoomCreatedAsync(new
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

        public async Task<List<WaitingRoomInfoDto>> GetWaitingRoomSummariesAsync()
        {
            var all = await _waitingRoomRepository.GetAllAsync();

            var result = new List<WaitingRoomInfoDto>();

            foreach (var wr in all.Where(wr => wr.Players.Count < GameRules.MaxPlayers))
            {
                var creator = await _playerRepository.GetByIdAsync(wr.CreatedByPlayerId);
                var creatorName = creator?.User?.Username ?? "Neznámý";

                result.Add(new WaitingRoomInfoDto
                {
                    WaitingRoomId = wr.Id,
                    CreatedAt = wr.CreatedAt,
                    CreatedBy = creatorName,
                    PlayerCount = wr.Players.Count
                });
            }

            return result;
        }
    }
}
