using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Logic;
using SupremeCourt.Domain.Sessions;
using SupremeCourt.Domain.Mappings;
using SupremeCourt.Application.Sessions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SupremeCourt.Application.Services
{
    public class WaitingRoomService : IWaitingRoomService
    {
        private readonly IWaitingRoomRepository _waitingRoomRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IGameService _gameService;
        private readonly IWaitingRoomNotifier _waitingRoomNotifier;
        private readonly WaitingRoomSessionManager _sessionManager;
        private readonly WaitingRoomMapper _mapper;
        private readonly IWaitingRoomEventHandler _eventHandler;
        private readonly ILogger<WaitingRoomService> _logger;
        private readonly int _roomExpirationSeconds = 3*60;

        public WaitingRoomService(
            IWaitingRoomRepository waitingRoomRepository,
            IPlayerRepository playerRepository,
            IGameService gameService,
            IWaitingRoomNotifier waitingRoomNotifier,
            WaitingRoomSessionManager sessionManager,
            WaitingRoomMapper mapper,
            IWaitingRoomEventHandler eventHandler,
            ILogger<WaitingRoomService> logger,
            IConfiguration configuration)
        {
            _waitingRoomRepository = waitingRoomRepository;
            _playerRepository = playerRepository;
            _gameService = gameService;
            _waitingRoomNotifier = waitingRoomNotifier;
            _sessionManager = sessionManager;
            _mapper = mapper;
            _eventHandler = eventHandler;
            _logger = logger;
            _roomExpirationSeconds = configuration.GetValue<int>("WaitingRoom:ExpirationMinutes") * 60;
        }

        public async Task<WaitingRoom?> CreateWaitingRoomAsync(int createdByPlayerId)
        {
            var player = await _playerRepository.GetByIdAsync(createdByPlayerId);
            if (player == null) return null;

            var waitingRoom = new WaitingRoom();
            

            await _waitingRoomRepository.AddAsync(waitingRoom);

            // Vytvoření runtime session
            var session = _mapper.ToSession(waitingRoom);

            session.OnCountdownTick += async seconds =>
                await _eventHandler.HandleCountdownTickAsync(session.WaitingRoomId, seconds);

            session.OnRoomExpired += async roomId =>
                await _eventHandler.HandleRoomExpiredAsync(roomId);

            _sessionManager.AddSession(session);

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
            var existingRoom = await _waitingRoomRepository.GetRoomByPlayerIdAsync(playerId);
            if (existingRoom != null) return false;

            var room = await _waitingRoomRepository.GetByIdAsync(waitingRoomId);
            if (room == null) return false;

            if (room.Players.Count >= GameRules.MaxPlayers)
            {
                _logger.LogWarning("Room {RoomId} is full", waitingRoomId);
                return false;
            }

            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null) return false;

            room.Players.Add(player);
            await _waitingRoomRepository.UpdateAsync(room);

            var session = _sessionManager.GetSession(waitingRoomId);
            if (session != null)
            {
                session.AddPlayer(player);
                await _waitingRoomNotifier.NotifyRoomUpdatedAsync(_mapper.ToDto(session));
            }

            return true;
        }

        public async Task<List<WaitingRoom>> GetAllWaitingRoomsAsync()
        {
            return await _waitingRoomRepository.GetAllAsync();
        }

        public async Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync()
        {
            var rooms = await _waitingRoomRepository.GetAllAsync();

            var list = new List<WaitingRoomDto>();
            foreach (var room in rooms.Where(r => r.Players.Count < GameRules.MaxPlayers))
            {
                var creator = await _playerRepository.GetByIdAsync(room.CreatedByPlayerId);
                list.Add(new WaitingRoomDto
                {
                    WaitingRoomId = room.Id,
                    CreatedAt = room.CreatedAt,
                    CreatedByPlayerId = room.CreatedByPlayerId,
                    Players = room.Players
                        .Select(p => new PlayerDto { PlayerId = p.Id, Username = p.User?.Username ?? "?" })
                        .ToList()
                });
            }

            return list;
        }

        public async Task<WaitingRoomDto?> GetWaitingRoomByIdAsync(int id)
        {
            var room = await _waitingRoomRepository.GetByIdAsync(id);
            if (room == null) return null;

            var session = _sessionManager.GetSession(id);
            var dto = _mapper.ToDto(room);
            dto.TimeLeftSeconds = session?.GetTimeLeft() ?? 0;

            return dto;
        }
    }
}
