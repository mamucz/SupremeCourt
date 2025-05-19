using MediatR;
using SupremeCourt.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public class AddAiPlayersBulkCommandHandler : IRequestHandler<AddAiPlayersBulkCommand, Unit>
    {
        private readonly IWaitingRoomSessionManager _sessionManager;

        public AddAiPlayersBulkCommandHandler(IWaitingRoomSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public async Task<Unit> Handle(AddAiPlayersBulkCommand request, CancellationToken cancellationToken)
        {
            var session = _sessionManager.GetSession(request.WaitingRoomId);
            if (session == null)
                throw new Exception("Waiting room not found");

            for (int i = 0; i < request.Count; i++)
            {
                await session.AddAiPlayerAsync(request.Type); // nebo jiný způsob přidání AI
            }

            return Unit.Value;
        }
    }

}
