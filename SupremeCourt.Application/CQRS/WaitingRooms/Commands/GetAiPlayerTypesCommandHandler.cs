using MediatR;
using SupremeCourt.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SupremeCourt.Application.CQRS.WaitingRooms.Commands.GetAiPlayerTypesCommand;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public class GetAiPlayerTypesCommandHandler : IRequestHandler<GetAiPlayerTypesCommand, List<AiPlayerTypeDto>>
    {
        private readonly IAIPlayerFactory _aiFactory;

        public GetAiPlayerTypesCommandHandler(IAIPlayerFactory aiFactory)
        {
            _aiFactory = aiFactory;
        }

        public async Task<List<AiPlayerTypeDto>> Handle(GetAiPlayerTypesCommand request, CancellationToken cancellationToken)
        {
            return await _aiFactory.GetAiPlayerTypesAsync();
        }
    }
}
