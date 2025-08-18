using MediatR;
using SupremeCourt.Domain.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static SupremeCourt.Application.CQRS.WaitingRooms.Commands.GetAiPlayerTypesCommand;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public class GetAiPlayerTypesCommandHandler : IRequestHandler<GetAiPlayerTypesQuery, List<AiPlayerTypeDto>>
    {
        private readonly IAiPlayerFactory _aiFactory;

        public GetAiPlayerTypesCommandHandler(IAiPlayerFactory aiFactory)
        {
            _aiFactory = aiFactory;
        }

        public async Task<List<AiPlayerTypeDto>> Handle(GetAiPlayerTypesQuery request,
            CancellationToken cancellationToken)
        {
            var types = await _aiFactory.GetAiPlayerTypesAsync();
            return types.Select(t => new AiPlayerTypeDto { Type = t }).ToList();
        }
    }
}