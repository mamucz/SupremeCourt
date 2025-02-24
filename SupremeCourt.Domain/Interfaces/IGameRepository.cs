using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IGameRepository
    {
        Task<Game?> GetByIdAsync(int id);
        Task AddAsync(Game game); // Přidáno
        Task UpdateAsync(Game game);
        Task<Game?> GetActiveGameByPlayerIdAsync(int playerId); // Přidáno
    }
}