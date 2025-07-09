using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IPlayerRepository
    {
        Task<Player?> GetByIdAsync(int id);
        Task<Player?> GetByUserIdAsync(int userId);
        Task AddAsync(Player player);
        Task DeleteAsync(Player player);
        Task<List<Player>> GetAllAsync();
        Task UpdateAsync(Player player); // Přidáno
        Task<List<Player>> GetAllAiPlayersAsync(CancellationToken cancellationToken);
        Task EnsureAiPlayerExistsAsync(string type);
    }
}