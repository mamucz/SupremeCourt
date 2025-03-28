using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Application.Services;
using System.Security.Claims;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces; // ✅ Přidáme using pro správnou definici UserDto


namespace SupremeCourt.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly TokenBlacklistService _tokenBlacklistService;

        public AuthController(IAuthService authService, TokenBlacklistService tokenBlacklistService)
        {
            _authService = authService;
            _tokenBlacklistService = tokenBlacklistService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto user)
        {
            var success = await _authService.RegisterAsync(user.Username, user.Password);
            if (!success)
                return BadRequest(new { message = "User already exists." });

            return Ok(new { message = "Registration successful" }); // ✅ objekt s klíčem message
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto user)
        {
            var token = await _authService.AuthenticateAsync(user.Username, user.Password);
            var userEntity = await _authService.GetUserByUsernameAsync(user.Username); // nový řádek

            if (token == null || userEntity == null)
                return Unauthorized(new { message = "Invalid username or password." });

            return Ok(new
            {
                message = "Login successful",
                token = token,
                userId = userEntity.Id // přidáno ID uživatele
            });
        }


        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "No token provided." });

            _tokenBlacklistService.BlacklistToken(token);

            return Ok(new { message = "Logged out successfully." });
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
                return NotFound(new {message = "User not found."});

            return Ok(new { message = "User deleted successfully." });
        }
    }
}