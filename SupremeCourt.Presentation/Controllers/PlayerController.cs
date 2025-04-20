using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Application.CQRS.Players.Commands;
using SupremeCourt.Application.CQRS.WaitingRooms.Queries;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using System.Security.Claims;
using SupremeCourt.Application.CQRS.Auth.DTOs;
using SupremeCourt.Application.CQRS.Players.Queries;

namespace SupremeCourt.Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;

        public PlayerController(IMediator mediator, IUserRepository userRepository)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Retrieves the actual state of a player (online, in waiting room, in game).
        /// </summary>
        /// <param name="id">The ID of the player (userId).</param>
        /// <returns>200 OK with player's current state.</returns>
        [HttpGet("playeractualstate/{id}")]
        [ProducesResponseType(typeof(ActualPlayerStateDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPlayerActualState(int id)
        {
            var result = await _mediator.Send(new GetActualPlayerStateQuery(id));
            return Ok(result);
        }

        /// <summary>
        /// Vrátí profilový obrázek uživatele (pokud existuje).
        /// </summary>
        /// <param name="id">ID uživatele</param>
        /// <returns>Soubor obrázku nebo 404 pokud neexistuje</returns>
        [AllowAnonymous]
        [HttpGet("{id}/profile-picture")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfilePicture(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.ProfilePicture == null)
                return NotFound();

            var mime = string.IsNullOrWhiteSpace(user.ProfilePictureMimeType) ? "image/jpeg" : user.ProfilePictureMimeType;
            return File(user.ProfilePicture, mime);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _mediator.Send(new ChangePasswordCommand(dto.UserId, dto.CurrentPassword, dto.NewPassword));

            if (!success)
                return BadRequest("Nesprávné aktuální heslo.");

            return Ok();
        }

    }

}