using System.Threading.Tasks;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string password);
        Task<bool> DeleteUserAsync(string username, string token);
        Task<User?> GetUserByUsernameAsync(string username);
        string GenerateJwtToken(User user);
        Task<RefreshToken> GenerateRefreshTokenAsync(int playerId);
    }
}