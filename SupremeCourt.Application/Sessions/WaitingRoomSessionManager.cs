using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Sessions;
using System.Collections.Concurrent;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.Sessions
{
    public class WaitingRoomSessionManager
    {
        private readonly ConcurrentDictionary<int, WaitingRoomSession> _sessions = new();
        private readonly IWaitingRoomEventHandler _eventHandler;

        public WaitingRoomSessionManager(IWaitingRoomEventHandler eventHandler)
        {
            _eventHandler = eventHandler;
        }

        public void AddSession(WaitingRoomSession session)
        {
            _sessions[session.WaitingRoomId] = session;

            session.OnCountdownTick += async seconds =>
            {
                await _eventHandler.HandleCountdownTickAsync(session.WaitingRoomId, seconds);
            };

            session.OnRoomExpired += async roomId =>
            {
                await _eventHandler.HandleRoomExpiredAsync(roomId);
            };
        }

        public WaitingRoomSession? GetSession(int roomId) =>
            _sessions.TryGetValue(roomId, out var session) ? session : null;

        public void RemoveSession(int roomId) =>
            _sessions.TryRemove(roomId, out _);

        public IEnumerable<WaitingRoomSession> GetAllSessions() => _sessions.Values;
    }


}
