using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SupremeCourt.Application.CQRS.Players.Commands
{
      
    public record AddAiPlayerToRoomCommand(int WaitingRoomId) : IRequest<bool>;

}
