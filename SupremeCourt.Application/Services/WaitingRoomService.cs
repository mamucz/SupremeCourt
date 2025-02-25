using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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

        public async Task<WaitingRoom?> CreateWaitingRoomAsync(int gameId)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null) return null;

            var waitingRoom = new WaitingRoom { GameId = gameId };
            await _waitingRoomRepository.AddAsync(waitingRoom);
            return waitingRoom;
        }

        public async Task<bool> JoinWaitingRoomAsync(int gameId, int playerId)
        {
            var waitingRoom = await _waitingRoomRepository.GetByGameIdAsync(gameId);
            if (waitingRoom == null) return false;

            if (waitingRoom.Players.Count >= GameRules.MaxPlayers) // ✅ Použití konstanty z Domain
            {
                _logger.LogWarning($"Hráč {playerId} se pokusil připojit do plné hry {gameId}.");
                return false;
            }

            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null) return false;

            waitingRoom.Players.Add(player);
            await _waitingRoomRepository.UpdateAsync(waitingRoom);

            if (waitingRoom.Players.Count == GameRules.MaxPlayers) // ✅ Použití pravidla z Domain
            {
                _logger.LogInformation($"Hra {gameId} má 5 hráčů, spouštíme ji.");
                return await _gameService.StartGameAsync(gameId);
            }
            // 🟢 Posíláme notifikaci přes SignalR všem hráčům v dané hře
            // 🟢 Použití notifieru místo přímého volání SignalR
            await _waitingRoomNotifier.NotifyPlayerJoinedAsync(gameId, player.User.Username);

            return true;
        }


        public async Task<bool> IsTimeExpiredAsync(int gameId)
        {
            var waitingRoom = await _waitingRoomRepository.GetByGameIdAsync(gameId);
            if (waitingRoom == null) return false;

            return DateTime.UtcNow > waitingRoom.CreatedAt.AddMinutes(1);
        }

        public async Task<List<WaitingRoom>> GetAllWaitingRoomsAsync() // ✅ Přidáno
        {
            return await _waitingRoomRepository.GetAllAsync();
        }
    }
}
