using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Mappings;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public class GetWaitingRoomByPlayerIdHandler : IRequestHandler<GetWaitingRoomByPlayerIdQuery, WaitingRoomDto?>
    {
        private readonly IWaitingRoomSessionManager _sessionManager;

        public GetWaitingRoomByPlayerIdHandler(IWaitingRoomSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public Task<WaitingRoomDto?> Handle(GetWaitingRoomByPlayerIdQuery request, CancellationToken cancellationToken)
        {
            var session = _sessionManager.GetSessionByPlayerId(request.PlayerId);
            if (session == null)
                return Task.FromResult<WaitingRoomDto?>(null);

            var dto = WaitingRoomSessionMapper.ToDto(session);
            return Task.FromResult<WaitingRoomDto?>(dto);
        }
    }

}
