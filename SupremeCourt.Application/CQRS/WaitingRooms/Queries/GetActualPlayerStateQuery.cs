using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Queries
{
    public class GetActualPlayerStateQuery : IRequest<ActualPlayerStateDto>
    {
        public int UserId { get; }

        public GetActualPlayerStateQuery(int userId)
        {
            UserId = userId;
        }
    }
}
