using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.Players.Commands
{
    public class CreatePlayerHandler
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreatePlayerHandler> _logger;
        public CreatePlayerHandler(IPlayerRepository playerRepository, IUserRepository userRepository, ILogger<CreatePlayerHandler> logger)
        {
            _playerRepository = playerRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Player?> Handle(CreatePlayerCommand command)
        {
            _logger.LogInformation("Pokus o vytvoření hráče pro UserId: {UserId}", command.UserId);

            // Ověření, zda uživatel existuje
            var user = await _userRepository.GetByIdAsync(command.UserId);
            if (user == null)
            {
                _logger.LogWarning("Uživatel s ID {UserId} neexistuje", command.UserId);
                return null; // Vrací null, pokud uživatel neexistuje
            }

            // Ověření, zda hráč pro tohoto uživatele již neexistuje
            var existingPlayer = await _playerRepository.GetByUserIdAsync(command.UserId);
            if (existingPlayer != null)
            {
                _logger.LogWarning("Hráč pro UserId {UserId} již existuje", command.UserId);
                return null; // Zabrání vytváření duplicitních hráčů
            }

            var player = new Player
            {
                UserId = command.UserId,
                Score = 0,
                IsEliminated = false,
                User = user
            };

            await _playerRepository.AddAsync(player);
            _logger.LogInformation("Hráč pro UserId {UserId} byl úspěšně vytvořen", command.UserId);

            return player;
        }
    }
}