using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.Players.Commands
{
    public class CreatePlayerHandler
    {
        private readonly IPlayerRepository _playerRepository;

        public CreatePlayerHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<Player> Handle(CreatePlayerCommand command)
        {
            var player = new Player { Name = command.Name, Score = 0 };
            await _playerRepository.AddAsync(player);
            return player;
        }
    }
}