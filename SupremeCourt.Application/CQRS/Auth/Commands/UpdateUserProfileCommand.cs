using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.Auth.Commands
{
    public class UpdateUserProfileCommand : IRequest<bool>
    {
        public string? Nickname { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public int UserId { get; set; }
    }
}
