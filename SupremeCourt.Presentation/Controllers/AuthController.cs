using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Application.Services;
using System.Security.Claims;

namespace SupremeCourt.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly TokenBlacklistService _tokenBlacklistService;

        public AuthController(AuthService authService, TokenBlacklistService tokenBlacklistService)
        {
            _authService = authService;
            _tokenBlacklistService = tokenBlacklistService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto user)
        {
            var success = await _authService.RegisterAsync(user.Username, user.Password);
            if (!success)
                return BadRequest("User already exists.");

            return Ok("Registration successful");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto user)
        {
            var token = await _authService.AuthenticateAsync(user.Username, user.Password);
            if (token == null)
                return Unauthorized();

            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
                return BadRequest("No token provided.");

            _tokenBlacklistService.BlacklistToken(token);
            return Ok("Logged out successfully.");
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
                return BadRequest("Invalid request.");

            var success = await _authService.DeleteUserAsync(username, token);
            if (!success)
                return NotFound("User not found.");

            return Ok("User deleted successfully.");
        }
    }

    public record UserDto(string Username, string Password);
}