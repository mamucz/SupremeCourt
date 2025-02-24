using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Application.Games.Commands;
using SupremeCourt.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace SupremeCourt.Presentation.Controllers
{
    [Route("api/game")]
    [ApiController]
    [Authorize] // Vyžaduje autorizaci pomocí JWT
    public class GameController : ControllerBase
    {
        private readonly CreateGameHandler _createGameHandler;
        private readonly ILogger<GameController> _logger;

        public GameController(CreateGameHandler createGameHandler, ILogger<GameController> logger)
        {
            _createGameHandler = createGameHandler;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
        {
            _logger.LogInformation("Požadavek na vytvoření hry od uživatele ID: {HostUserId}", request.HostUserId);

            var command = new CreateGameCommand { HostUserId = request.HostUserId };
            var game = await _createGameHandler.Handle(command);

            return Ok(new { message = "Hra vytvořena", gameId = game.Id });
        }
    }

    public class CreateGameRequest
    {
        public int HostUserId { get; set; } // ID uživatele, který zakládá hru
    }
}