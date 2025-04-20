using MediatR;
using SupremeCourt.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.Players.Queries
{
    public record GetUserProfileQuery(int UserId) : IRequest<UserProfileDto>;
}
