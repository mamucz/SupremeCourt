using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.Players.Commands
{
      
    public record AddAiPlayerToRoomCommand(Guid WaitingRoomId, IPlayer player) : IRequest<bool>;

}
