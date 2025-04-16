using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SupremeCourt.Application.CQRS.Auth.Commands
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<string?>;

}
