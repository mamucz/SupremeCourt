using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.Services
{
    public class WaitingRoomService : IWaitingRoomService
    {
        private readonly IWaitingRoomRepository _waitingRoomRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<WaitingRoomService> _logger;

        public WaitingRoomService(
            IWaitingRoomRepository waitingRoomRepository,
            IGameRepository gameRepository,
            IPlayerRepository playerRepository,
            ILogger<WaitingRoomService> logger)
        {
            _waitingRoomRepository = waitingRoomRepository;
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
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

            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null) return false;

            waitingRoom.Players.Add(player);
            await _waitingRoomRepository.UpdateAsync(waitingRoom);
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
