using SupremeCourt.Domain.Interfaces;
using System.Collections.Concurrent;

namespace SupremeCourt.Application.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();

        public void BlacklistToken(string token)
        {
            _blacklistedTokens[token] = DateTime.UtcNow.AddHours(60); // Token bude zablokován
        }

        public bool IsTokenBlacklisted(string token)
        {
            return _blacklistedTokens.ContainsKey(token);
        }
    }
}