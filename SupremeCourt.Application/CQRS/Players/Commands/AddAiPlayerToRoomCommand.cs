using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.Players.Commands
{

    public class AddAiPlayerToRoomCommand : IRequest<bool>
    {
        public Guid WaitingRoomId { get; set; }
        public string Type { get; set; } = default!;
    }

}
