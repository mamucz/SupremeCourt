using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Application.Services;
using System.Security.Claims;
using MediatR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Application.CQRS.Auth.Commands;

namespace SupremeCourt.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly TokenBlacklistService _tokenBlacklistService;
        private readonly IMediator _mediator;

        public AuthController(
            IAuthService authService,
            TokenBlacklistService tokenBlacklistService,
            IMediator mediator)
        {
            _authService = authService;
            _tokenBlacklistService = tokenBlacklistService;
            _mediator = mediator;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="command">The user registration request containing username and password.</param>
        /// <returns>
        /// 200 OK if the registration is successful.  
        /// 409 Conflict if a user with the same username already exists.
        /// </returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var success = await _mediator.Send(command);

            if (!success)
                return Conflict(new { message = "User already exists." });

            return Ok(new { message = "Registration successful." });
        }

        /// <summary>
        /// Authenticates the user and returns a JWT token and user ID.
        /// </summary>
        /// <param name="command">The login request containing username and password.</param>
        /// <returns>
        /// 200 OK with token and userId if authentication succeeds.  
        /// 401 Unauthorized if credentials are invalid.
        /// </returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (result == null)
                return Unauthorized(new { message = "Invalid username or password." });

            return Ok(new
            {
                message = "Login successful.",
                token = result.Token,
                userId = result.UserId
            });
        }

        /// <summary>
        /// Logs out the current user by blacklisting the provided token.
        /// </summary>
        /// <returns>
        /// 200 OK if the token was successfully blacklisted.  
        /// 400 Bad Request if the token is missing or invalid.
        /// </returns>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var command = new LogoutUserCommand { Token = token };
            var success = await _mediator.Send(command);

            if (!success)
                return BadRequest(new { message = "No token provided." });

            return Ok(new { message = "Logged out successfully." });
        }

        /// <summary>
        /// Deletes the currently authenticated user.
        /// </summary>
        /// <returns>
        /// 200 OK if the user was deleted.  
        /// 400 Bad Request if the request is invalid.  
        /// 404 Not Found if the user does not exist.
        /// </returns>
        [Authorize]
        [HttpDelete("delete")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Invalid request." });

            var command = new DeleteUserCommand
            {
                Username = username,
                Token = token
            };

            var success = await _mediator.Send(command);
            if (!success)
                return NotFound(new { message = "User not found." });

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
