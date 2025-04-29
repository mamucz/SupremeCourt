using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IGameService
    {
        Task<Game?> CreateGameAsync();
        Task<Game?> GetGameByIdAsync(int gameId);
        Task<GameRound> StartNewRound(int gameId, Dictionary<int, int> playerChoices);
        Task<Game> StartGameAsync(List<IPlayer> players);
        Task<Game?> GetGameIdByUserIdAsync(int playerId);
    }
}