using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.Players.Queries
{
    public class GetAllAiPlayersQueryHandler : IRequestHandler<GetAllAiPlayersQuery, List<PlayerDto>>
    {
        private readonly IPlayerRepository _playerRepository;

        public GetAllAiPlayersQueryHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<List<PlayerDto>> Handle(GetAllAiPlayersQuery request, CancellationToken cancellationToken)
        {
            var aiPlayers = await _playerRepository.GetAllAiPlayersAsync(cancellationToken);

            return aiPlayers.Select(player => new PlayerDto
            {
                PlayerId = player.Id,
                Username = player.Username,
               
            }).ToList();
        }
    }

}
