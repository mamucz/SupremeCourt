using MediatR;
using Microsoft.AspNetCore.Http;
using SupremeCourt.Application.Services;
using SupremeCourt.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.Players.Commands
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenBlacklistService _tokenBlacklist;

        public ChangePasswordCommandHandler(IUserRepository userRepository, IHttpContextAccessor contextAccessor, ITokenBlacklistService tokenBlacklistService)
        {
            _userRepository = userRepository;
            _httpContextAccessor = contextAccessor;
            _tokenBlacklist = tokenBlacklistService;
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            // Vše OK → aktualizuj na nové heslo
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .ToString().Replace("Bearer ", "");

            if (!string.IsNullOrWhiteSpace(token))
            {
                _tokenBlacklist.BlacklistToken(token);
            }
            return true;
        }
    }

}
