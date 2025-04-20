using MediatR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Application.CQRS.Players.Queries
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IUserRepository _userRepository;

        public GetUserProfileQueryHandler(IPlayerRepository playerRepository, IUserRepository userRepository)
        {
            _playerRepository = playerRepository;
            _userRepository = userRepository;
        }

        public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var player = await _playerRepository.GetByIdAsync(request.UserId);
            var user = await _userRepository.GetByIdAsync(request.UserId);
            return new UserProfileDto
            {
                UserName = user.Username,
                ProfileImage = user.ProfilePicture,
                ProfileImageMimeType = user.ProfilePictureMimeType,
                PasswordHash = user.PasswordHash,
            };
        }
    }

}
