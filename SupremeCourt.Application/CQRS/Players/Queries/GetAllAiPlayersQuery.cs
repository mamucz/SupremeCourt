using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Application.CQRS.Players.Queries
{
    public record GetAllAiPlayersQuery() : IRequest<List<PlayerDto>>;
}
