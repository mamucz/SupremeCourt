using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Sessions;

namespace SupremeCourt.Application.Services
{
    public class WaitingRoomSessionManager : IWaitingRoomSessionManager
    {
        private readonly ConcurrentDictionary<int, WaitingRoomSession> _sessions = new();
        private readonly ILogger<WaitingRoomSessionManager> _logger;
        private readonly IWaitingRoomEventHandler _eventHandler;

        public WaitingRoomSessionManager(
            ILogger<WaitingRoomSessionManager> logger,
            IWaitingRoomEventHandler eventHandler)
        {
            _logger = logger;
            _eventHandler = eventHandler;
        }

        public WaitingRoomSession? GetSession(int roomId)
        {
            _sessions.TryGetValue(roomId, out var session);
            return session;
        }

        public List<WaitingRoomSession> GetAllSessions() => _sessions.Values.ToList();

        public bool TryJoinPlayer(int roomId, IPlayer player)
        {
            if (!_sessions.TryGetValue(roomId, out var session))
                return false;

            var result = session.TryAddPlayer(player);

            if (result)
            {
                _logger.LogInformation("Hráč {PlayerId} se připojil do místnosti {RoomId}", player.Id, roomId);
                _ = _eventHandler.NotifyPlayerJoinedAsync(roomId, new PlayerDto
                {
                    PlayerId = player.Id,
                    Username = player.Username,
                   
                }, CancellationToken.None);
            }

            return result;
        }

        public int CreateRoom(IPlayer createdBy)
        {
            var roomId = GenerateRoomId();
            var session = new WaitingRoomSession(roomId, createdBy, _eventHandler);
            if (_sessions.TryAdd(roomId, session))
            {
                _logger.LogInformation("Vytvořena nová místnost {RoomId} hráčem {PlayerId}", roomId, createdBy.Id);
            }
            return roomId;
        }

        public void RemoveRoom(int roomId)
        {
            if (_sessions.TryRemove(roomId, out var session))
            {
                _logger.LogInformation("Místnost {RoomId} byla odstraněna", roomId);
                session.Dispose();
            }
        }

        private int GenerateRoomId()
        {
            var rand = new Random();
            int id;
            do
            {
                id = rand.Next(1000, 9999);
            } while (_sessions.ContainsKey(id));

            return id;
        }

        public bool TryRemovePlayer(int roomId, int playerId)
        {
            if (!_sessions.TryGetValue(roomId, out var session))
                return false;

            return session.TryRemovePlayer(playerId);
        }

        public WaitingRoomSession? GetSessionByPlayerId(int playerId)
        {
            return _sessions.Values.FirstOrDefault(session => session.Players.Any(p => p.Id == playerId));
        }

        public void RemoveSession(int roomId)
        {
            if (_sessions.TryRemove(roomId, out var session))
            {
                _logger.LogInformation("Místnost {RoomId} byla odstraněna", roomId);
                session.Dispose(); // Uvolní Timer atd.
            }
        }

    }
}
