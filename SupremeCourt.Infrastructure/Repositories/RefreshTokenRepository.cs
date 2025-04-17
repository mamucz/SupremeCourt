using Microsoft.EntityFrameworkCore;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly GameDbContext _context;

        public RefreshTokenRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> CreateAsync(int playerId, CancellationToken cancellationToken = default)
        {
            var token = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(7),
                PlayerId = playerId
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync(cancellationToken);
            return token;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == token && r.Expires > DateTime.UtcNow, cancellationToken);
        }

        public async Task InvalidateAsync(string token, CancellationToken cancellationToken = default)
        {
            var entity = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
            if (entity != null)
            {
                _context.RefreshTokens.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
