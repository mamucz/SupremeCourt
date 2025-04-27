using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Sessions;
using Microsoft.Extensions.Logging;

namespace SupremeCourt.Application.Services
{
    public class WaitingRoomListService : IWaitingRoomListService
    {
        private readonly IWaitingRoomSessionManager _sessionManager;
        private readonly ILogger<WaitingRoomListService> _logger;

        public WaitingRoomListService(
            IWaitingRoomSessionManager sessionManager,
            ILogger<WaitingRoomListService> logger)
        {
            _sessionManager = sessionManager;
            _logger = logger;
        }

        public async Task<WaitingRoomSession?> CreateWaitingRoomAsync(int createdByPlayerId, CancellationToken cancellationToken)
        {
            // TODO: Načti hráče z databáze přes IPlayerRepository
            // Zatím fake player
            var fakePlayer = new SupremeCourt.Domain.Entities.Player
            {
                Id = createdByPlayerId,
                User = new SupremeCourt.Domain.Entities.User
                {
                    Id = createdByPlayerId,
                    Username = $"Player {createdByPlayerId}"
                }
            };

            var roomId = _sessionManager.CreateRoom(fakePlayer as IPlayer);
            var session = _sessionManager.GetSession(roomId);

            return session;
        }

        public async Task<bool> JoinWaitingRoomAsync(int waitingRoomId, int playerId, CancellationToken cancellationToken)
        {
            // TODO: Načti hráče z databáze přes IPlayerRepository
            var fakePlayer = new SupremeCourt.Domain.Entities.Player
            {
                Id = playerId,
                User = new SupremeCourt.Domain.Entities.User
                {
                    Id = playerId,
                    Username = $"Player {playerId}"
                }
            };

            return _sessionManager.TryJoinPlayer(waitingRoomId, fakePlayer as IPlayer);
        }

        public async Task<List<WaitingRoomSession>> GetAllWaitingRoomsAsync(CancellationToken cancellationToken)
        {
            return _sessionManager.GetAllSessions();
        }

        public async Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync(CancellationToken cancellationToken)
        {
            var sessions = _sessionManager.GetAllSessions();

            var result = sessions.Select(session => new WaitingRoomDto
            {
                WaitingRoomId = session.WaitingRoomId,
                CreatedAt = session.CreatedAt,
                CreatedByPlayerId = session.CreatedBy.Id,
                TimeLeftSeconds = session.GetTimeLeft()
            }).ToList();

            return result;
        }
    }
}
