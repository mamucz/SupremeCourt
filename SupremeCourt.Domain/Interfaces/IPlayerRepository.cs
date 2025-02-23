using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IPlayerRepository
    {
        Task<Player?> GetByIdAsync(int id);
        Task AddAsync(Player player);
        Task<List<Player>> GetAllAsync();
    }
}