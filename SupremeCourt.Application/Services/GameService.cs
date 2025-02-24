using System;
using System.Linq;
using System.Collections.Generic;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace SupremeCourt.Application.Services
{
    public class GameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<GameService> _logger;

        public GameService(IGameRepository gameRepository, IPlayerRepository playerRepository, ILogger<GameService> logger)
        {
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<GameRound> StartNewRound(int gameId, Dictionary<int, int> playerChoices)
        {
            _logger.LogInformation("Spouštím nové kolo pro hru ID: {GameId}", gameId);

            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null || !game.IsActive)
            {
                _logger.LogWarning("Hra ID {GameId} neexistuje nebo není aktivní", gameId);
                throw new InvalidOperationException("Hra neexistuje nebo není aktivní.");
            }

            // Výpočet zaokrouhleného průměru * 0.8
            double average = playerChoices.Values.Average();
            int calculatedAverage = (int)Math.Round(average * 0.8);

            // Hledání hráče s nejbližší hodnotou k výsledku
            int winningPlayerId = playerChoices
                .OrderBy(p => Math.Abs(p.Value - calculatedAverage))
                .First().Key;

            // Vytvoření nového kola
            var round = new GameRound
            {
                GameId = gameId,
                RoundNumber = game.RoundNumber,
                PlayerChoices = playerChoices,
                CalculatedAverage = calculatedAverage,
                WinningPlayerId = winningPlayerId
            };

            // Snížení skóre hráčům, kteří nevyhráli
            foreach (var playerId in playerChoices.Keys)
            {
                if (playerId != winningPlayerId)
                {
                    var player = await _playerRepository.GetByIdAsync(playerId);
                    if (player != null)
                    {
                        player.Score -= 1;
                        if (player.Score <= -10)
                        {
                            player.IsEliminated = true;
                        }
                        await _playerRepository.UpdateAsync(player);
                    }
                }
            }

            game.RoundNumber++;
            await _gameRepository.UpdateAsync(game);

            _logger.LogInformation("Kolo {RoundNumber} dokončeno. Vítězem je hráč ID {WinningPlayerId}", round.RoundNumber, winningPlayerId);

            return round;
        }
    }
}
