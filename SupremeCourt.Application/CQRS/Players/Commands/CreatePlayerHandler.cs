using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.Players.Commands
{
    public class CreatePlayerHandler
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<CreatePlayerHandler> _logger;
        public CreatePlayerHandler(IPlayerRepository playerRepository,  ILogger<CreatePlayerHandler> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<Player?> Handle(CreatePlayerCommand command)
        {
            _logger.LogInformation("Pokus o vytvoření hráče : {UserId}", command.User.Username);

            var player = new Player(command.User);

            _logger.LogInformation("Hráč {UserName} byl úspěšně vytvořen", command.User.Username);
            return player;
        }
    }
}