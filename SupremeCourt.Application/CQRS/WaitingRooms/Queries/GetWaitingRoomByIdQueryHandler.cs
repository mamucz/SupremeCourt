using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Mappings;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Queries
{
    public class GetWaitingRoomByIdQueryHandler : IRequestHandler<GetWaitingRoomByIdQuery, WaitingRoomDto?>
    {
        private readonly IWaitingRoomRepository _repository;
        
        public GetWaitingRoomByIdQueryHandler(IWaitingRoomRepository repository)
        {
            _repository = repository;
        }

        public async Task<WaitingRoomDto?> Handle(GetWaitingRoomByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetByIdAsync(request.WaitingRoomId, cancellationToken);

            return Domain.Mappings.WaitingRoomMapper.Instance.ToDto(result);
        }
    }
}
