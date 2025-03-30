using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.DTOs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.Games.Commands
{
    public class CreateGameHandler : ICreateGameHandler
    {
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<CreateGameHandler> _logger;

        public CreateGameHandler(IGameRepository gameRepository, IPlayerRepository playerRepository, ILogger<CreateGameHandler> logger)
        {
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<Game?> HandleAsync(CreateGameDto dto) // ✅ Použití CreateGameDto
        {
            _logger.LogInformation("Požadavek na vytvoření hry od uživatele ID: {HostUserId}", dto.HostUserId);

            // Ověření, zda hráč existuje
            var player = await _playerRepository.GetByUserIdAsync(dto.HostUserId);
            if (player == null)
            {
                _logger.LogWarning("Uživatel ID {HostUserId} nemá hráčský účet", dto.HostUserId);
                throw new InvalidOperationException("Uživatel nemá hráčský účet.");
            }

            // Ověření, zda hráč už není v aktivní hře
            var activeGame = await _gameRepository.GetActiveGameByPlayerIdAsync(player.Id);
            if (activeGame != null)
            {
                _logger.LogWarning("Hráč ID {PlayerId} už je ve hře ID {GameId}", player.Id, activeGame.Id);
                throw new InvalidOperationException("Hráč už je v jiné aktivní hře.");
            }

            // Vytvoření nové hry
            var game = new Game
            {
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                RoundNumber = 1,
                Players = new List<Player> { player }
            };

            await _gameRepository.AddAsync(game);
            _logger.LogInformation("Hra ID {GameId} byla úspěšně vytvořena hráčem ID {PlayerId}", game.Id, player.Id);

            return game;
        }
    }
}
