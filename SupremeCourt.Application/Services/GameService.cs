using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Logic;

namespace SupremeCourt.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<GameService> _logger;

        public GameService(
            IGameRepository gameRepository,
            IPlayerRepository playerRepository,
            ILogger<GameService> logger)
        {
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<Game?> CreateGameAsync()
        {
            var game = new Game();
            await _gameRepository.AddAsync(game); // tady AddAsync() musí buď:
            // - ihned SaveChanges
            // - nebo ty ho musíš explicitně zavolat
            _logger.LogInformation("🎯 Hra {GameId} byla vytvořena.", game.Id);
            return game;
        }

        public async Task<Game?> GetGameByIdAsync(int gameId)
        {
            return await _gameRepository.GetByIdAsync(gameId);
        }

        public async Task<GameRound> StartNewRound(int gameId, Dictionary<int, int> playerChoices)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null || !game.IsActive)
            {
                _logger.LogWarning("⚠️ Hra ID {GameId} neexistuje nebo není aktivní.", gameId);
                throw new InvalidOperationException("Hra neexistuje nebo není aktivní.");
            }

            GameRules.ProcessRound(game, playerChoices);

            await _gameRepository.UpdateAsync(game);

            return new GameRound
            {
                GameId = gameId,
                RoundNumber = game.RoundNumber,
                PlayerChoices = playerChoices,
                CalculatedAverage = (int)Math.Round(playerChoices.Values.Average() * 0.8),
                WinningPlayerId = playerChoices
                    .OrderBy(p => Math.Abs(p.Value - (int)Math.Round(playerChoices.Values.Average() * 0.8)))
                    .First().Key
            };
        }

        public async Task<Game> StartGameAsync(List<IPlayer> players)
        {
            var game = new Game
            {
                Players = players.Cast<Player>().ToList(), // Přetypujeme na Player
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };


            await _gameRepository.AddAsync(game);
            _logger.LogInformation("🚀 Nová hra {GameId} spuštěna.", game.Id);

            return game;
        }


        public async Task<Game?> GetGameIdByUserIdAsync(int playerId)
        {
            return await _gameRepository.GetActiveGameByPlayerIdAsync(playerId);
        }
    }
}
