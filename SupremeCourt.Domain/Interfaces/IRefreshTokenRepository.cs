using System.Threading;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> CreateAsync(int playerId, CancellationToken cancellationToken = default);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task InvalidateAsync(string token, CancellationToken cancellationToken = default);
    }
}
