using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;


namespace SupremeCourt.Application.Services
{
    public class WaitingRoomListService : IWaitingRoomListService
    {
        private readonly IWaitingRoomSessionManager _sessionManager;

        public WaitingRoomListService(IWaitingRoomSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync(CancellationToken cancellationToken)
        {
            var sessions = _sessionManager.GetAllSessions();
            var summaries = sessions.Select(s => new WaitingRoomDto
            {
                WaitingRoomId = s.WaitingRoomId,
                CreatedAt = s.CreatedAt,
                CreatedByPlayerId = s.CreatedBy.Id,
                CreatedByPlayerName = s.CreatedBy.Username,
                Players = s.Players.Select(p => new PlayerDto
                {
                    PlayerId = p.Id,
                    Username = p.Username
                }).ToList(),
                TimeLeftSeconds = s.GetTimeLeft()
            }).ToList();

            return Task.FromResult(summaries);
        }

        public Task<List<WaitingRoomSession>> GetAllWaitingRoomsAsync(CancellationToken cancellationToken)
        {
            var sessions = _sessionManager.GetAllSessions();
            return Task.FromResult(sessions);
        }

        public async Task<WaitingRoomSession?> CreateWaitingRoomAsync(int createdByPlayerId, CancellationToken cancellationToken)
        {
            // Tady by normálně bylo načtení hráče z repozitáře
            // My to teď jednoduše nahradíme "mock" hráčem
            var fakePlayer = new SupremeCourt.Domain.Entities.Player
            {
                Id = createdByPlayerId,
                Username = $"Player{createdByPlayerId}"
            };

            var roomId = _sessionManager.CreateRoom(fakePlayer);
            return _sessionManager.GetSession(roomId);
        }

        public Task<bool> JoinWaitingRoomAsync(Guid waitingRoomId, int playerId, CancellationToken cancellationToken)
        {
            var session = _sessionManager.GetSession(waitingRoomId);
            if (session == null)
                return Task.FromResult(false);

            var fakePlayer = new SupremeCourt.Domain.Entities.Player
            {
                Id = playerId,
                Username = $"Player{playerId}"
            };

            var result = _sessionManager.TryJoinPlayer(waitingRoomId, fakePlayer);
            return Task.FromResult(result);
        }
    }
}
