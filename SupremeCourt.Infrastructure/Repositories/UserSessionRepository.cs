using System.Collections.Concurrent;
using SupremeCourt.Application.Common.Interfaces;

namespace SupremeCourt.Infrastructure.Services
{
    public class UserSessionRepository : IUserSessionRepository
    {
        // In-memory úložiště: userId → poslední aktivita
        private readonly ConcurrentDictionary<int, DateTime> _connectedUsers = new();

        public bool IsUserConnected(int userId)
        {
            return _connectedUsers.ContainsKey(userId);
        }

        public void AddUser(int userId)
        {
            _connectedUsers[userId] = DateTime.UtcNow;
        }

        public void RemoveUser(int userId)
        {
            _connectedUsers.TryRemove(userId, out _);
        }

        public IReadOnlyCollection<int> GetAllConnectedUsers()
        {
            return _connectedUsers.Keys.ToList().AsReadOnly();
        }

    }
}
