using SupremeCourt.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Application.Services
{
    class RegistrAIPlayersService
    {
        private readonly IPlayerRepository _playerRepository;

        public RegistrAIPlayersService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }
        public async Task RegisterAiPlayersAsync(CancellationToken cancellationToken)
        {
            var aiPlayerTypes = await _playerRepository.GetAllAiPlayersAsync(cancellationToken);
            if (aiPlayerTypes == null || !aiPlayerTypes.Any())
                return;
            foreach (var aiPlayerType in aiPlayerTypes)
            {
                await _playerRepository.EnsureAiPlayerExistsAsync(aiPlayerType);
            }
        }


    }
}
