using MediatR;
using SupremeCourt.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    class GetAiPlayerTypesCommand
    {
        public record GetAiPlayerTypesQuery : IRequest<List<AiPlayerTypeDto>>;
    }
}
