using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public record AddAiPlayersBulkCommand(Guid WaitingRoomId, int Count, string Type) : IRequest<Unit>;
}
