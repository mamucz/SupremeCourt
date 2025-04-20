using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.Players.Commands
{
    public record ChangePasswordCommand(int UserId, string CurrentPassword, string NewPassword) : IRequest<bool>;

}
