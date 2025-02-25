using System.Threading.Tasks;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string password);
        Task<bool> DeleteUserAsync(string username, string token); // ✅ Přidáno
    }
}