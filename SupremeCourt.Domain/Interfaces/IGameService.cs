using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IGameService
    {
        Task<Game?> CreateGameAsync();
        Task<Game?> GetGameByIdAsync(int gameId);
        Task<GameRound> StartNewRound(int gameId, Dictionary<int, int> playerChoices);
        Task<bool> StartGameAsync(int gameId);
    }
}