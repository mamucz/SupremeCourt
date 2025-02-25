using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Logic
{
    public static class GameRules
    {
        public const int MaxPlayers = 5;
        public const int MinScore = -10;

        public static void StartGame(Game game)
        {
            if (game.Players.Count < MaxPlayers)
                throw new InvalidOperationException("Game cannot start until all 5 players have joined.");

            game.IsActive = true;
            game.RoundNumber = 1;
        }

        public static void ProcessRound(Game game, Dictionary<int, int> playerChoices)
        {
            if (!game.IsActive)
                throw new InvalidOperationException("Game is not active.");

            // Výpočet průměru * 0.8
            double average = playerChoices.Values.Average();
            int calculatedAverage = (int)Math.Round(average * 0.8);

            // Hledání hráče s nejbližší hodnotou k výsledku
            int winningPlayerId = playerChoices
                .OrderBy(p => Math.Abs(p.Value - calculatedAverage))
                .First().Key;

            // Aktualizace skóre hráčů
            foreach (var player in game.Players)
            {
                if (player.Id != winningPlayerId)
                {
                    player.Score -= 1;
                    if (player.Score <= MinScore)
                    {
                        player.IsEliminated = true;
                    }
                }
            }

            game.RoundNumber++;
        }
    }
}