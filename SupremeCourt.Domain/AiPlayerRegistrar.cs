using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Domain
{
    public class AiPlayerRegistrar : IAIPlayerRegistrar
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPlayerRepository _playerRepository;
        private readonly IUserRepository _userRepository;

        public AiPlayerRegistrar(
            IServiceProvider serviceProvider,
            IPlayerRepository playerRepository,
            IUserRepository userRepository)
        {
            _serviceProvider = serviceProvider;
            _playerRepository = playerRepository;
            _userRepository = userRepository;
        }

        public async Task RegisterAllAiPlayersAsync(CancellationToken cancellationToken = default)
        {
            var aiTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && typeof(IAIPlayerDefinition).IsAssignableFrom(t));

            foreach (var type in aiTypes)
            {
                var ai = (IAIPlayerDefinition)Activator.CreateInstance(type)!;

                var exists = await _userRepository.GetByUsernameAsync(ai.Username);
                if (exists != null) continue;

                var user = new User
                {
                    Username = ai.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234")

                };

                await _userRepository.AddAsync(user);

                var player = new Player
                {
                    //User = user,
                    //IsAi = true, 
                };

                await _playerRepository.AddAsync(player);
            }
        }
    }

}
