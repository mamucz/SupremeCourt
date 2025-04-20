using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Sessions;
using System.Collections.Concurrent;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.Services
{
    public class WaitingRoomSessionManager
    {
        private readonly ConcurrentDictionary<int, WaitingRoomSession> _sessions = new();
        private readonly Lazy<IWaitingRoomEventHandler> _eventHandler;

        public WaitingRoomSessionManager(Lazy<IWaitingRoomEventHandler> eventHandler)
        {
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        }

        public void AddSession(WaitingRoomSession session)
        {
            _sessions[session.WaitingRoomId] = session;

            session.OnCountdownTick += async (roomId, secondsLeft) =>
            {
                await _eventHandler.Value.HandleCountdownTickAsync(roomId, secondsLeft);
            };

            session.OnRoomExpired += async roomId =>
            {
                await _eventHandler.Value.HandleRoomExpiredAsync(roomId);
            };
        }

        public WaitingRoomSession? GetSession(int roomId) =>
            _sessions.TryGetValue(roomId, out var session) ? session : null;

        public void RemoveSession(int roomId) =>
            _sessions.TryRemove(roomId, out _);

        public IEnumerable<WaitingRoomSession> GetAllSessions() => _sessions.Values;
    }
}