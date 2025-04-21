using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly IWaitingRoomListNotifier _waitingRoomListNotifier;
        private readonly IWaitingRoomNotifier _waitingRoomNotifier;
        private readonly WaitingRoomSessionManager _sessionManager;
        private readonly IWaitingRoomEventHandler _eventHandler;
        private readonly int _roomExpirationSeconds;

        public WaitingRoomListService(
            IWaitingRoomRepository waitingRoomRepository,
            IGameRepository gameRepository,
            IPlayerRepository playerRepository,
            IGameService gameService,
            IWaitingRoomListNotifier waitingRoomListNotifier,
            IWaitingRoomNotifier waitingRoomNotifier,
            WaitingRoomSessionManager sessionManager,
            IWaitingRoomEventHandler eventHandler,
            ILogger<WaitingRoomListService> logger,
            IConfiguration configuration)
        {
            _waitingRoomRepository = waitingRoomRepository;
            _playerRepository = playerRepository;
            _waitingRoomListNotifier = waitingRoomListNotifier;
            _waitingRoomNotifier = waitingRoomNotifier;
            _sessionManager = sessionManager;
            _eventHandler = eventHandler;
            _logger = logger;
            _roomExpirationSeconds = configuration.GetValue<int>("WaitingRoom:ExpirationMinutes", 3) * 60;
        }

        public async Task<WaitingRoom?> CreateWaitingRoomAsync(int createdByPlayerId, CancellationToken cancellationToken)
        {
            var player = await _playerRepository.GetByIdAsync(createdByPlayerId);
            if (player == null) return null;

            var waitingRoom = new WaitingRoom
            {
                CreatedByPlayerId = createdByPlayerId,
                Players = new List<Player>()
            };

            await _waitingRoomRepository.AddAsync(waitingRoom, cancellationToken);

            // ✅ Po uložení vytvoř runtime session a přidej ji do manageru
            var session = Domain.Mappings.WaitingRoomMapper.Instance.ToSession(waitingRoom);

            session.OnCountdownTick += async (roomId, secondsLeft) =>
            {
                await _eventHandler.HandleCountdownTickAsync(roomId, secondsLeft);
            };

            session.OnRoomExpired += async roomId =>
            {
                await _eventHandler.HandleRoomExpiredAsync(roomId);
            };

            session.InitializeFromEntity(waitingRoom, _roomExpirationSeconds);
            _sessionManager.AddSession(session);

            await _waitingRoomListNotifier.NotifyWaitingRoomCreatedAsync(new WaitingRoomDto
            {
                WaitingRoomId = waitingRoom.Id,
                CreatedAt = waitingRoom.CreatedAt,
                CreatedByPlayerId = player.User.Id,
                CreatedByPlayerName = player.User?.Username ?? "Neznámý",
                TimeLeftSeconds = _roomExpirationSeconds
            });

            return waitingRoom;
        }

        public async Task<bool> JoinWaitingRoomAsync(int waitingRoomId, int playerId, CancellationToken cancellationToken)
        {
            var existingRoom = await _waitingRoomRepository.GetRoomByPlayerIdAsync(playerId, cancellationToken);
            if (existingRoom != null)
            {
                _logger.LogWarning("Hráč {PlayerId} je již ve waiting room #{RoomId}", playerId, existingRoom.Id);
                return false;
            }

            var waitingRoom = await _waitingRoomRepository.GetByIdAsync(waitingRoomId, cancellationToken);
            if (waitingRoom == null) return false;

            if (waitingRoom.Players.Count >= GameRules.MaxPlayers)
            {
                _logger.LogWarning($"Hráč {playerId} se pokusil připojit do plné místnosti {waitingRoomId}.", cancellationToken);
                return false;
            }

            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null) return false;

            waitingRoom.Players.Add(player);
            await _waitingRoomRepository.UpdateAsync(waitingRoom, cancellationToken);

            await _waitingRoomNotifier.NotifyPlayerJoinedAsync(waitingRoomId, Domain.Mappings.PlayerMapper.Instance.ToDto(player));

            return true;
        }

        public async Task<List<WaitingRoom>> GetAllWaitingRoomsAsync(CancellationToken cancellationToken)
        {
            return await _waitingRoomRepository.GetAllAsync(cancellationToken);
        }

        public async Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync(CancellationToken cancellationToken)
        {
            var all = await _waitingRoomRepository.GetAllAsync(cancellationToken);

            var result = new List<WaitingRoomDto>();

            foreach (var wr in all.Where(wr => wr.Players.Count < GameRules.MaxPlayers))
            {
                var creator = await _playerRepository.GetByIdAsync(wr.CreatedByPlayerId);
                var creatorName = creator?.User?.Username ?? "Neznámý";
                var creatorUserId = creator?.User?.Id ?? -1;

                var session = _sessionManager.GetSession(wr.Id);
                var secondsLeft = session?.GetTimeLeft() ?? _roomExpirationSeconds;

                result.Add(new WaitingRoomDto
                {
                    WaitingRoomId = wr.Id,
                    CreatedAt = wr.CreatedAt,
                    CreatedByPlayerId = creatorUserId,
                    CreatedByPlayerName = creatorName,
                    TimeLeftSeconds = secondsLeft
                });
            }

            return result;
        }
    }
}
