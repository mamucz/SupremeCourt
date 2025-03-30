using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Domain.Entities;
using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.DTOs;
using System.Threading.Tasks;

namespace SupremeCourt.Presentation.Controllers
{
    [Route("api/game")]
    [ApiController]
    [Authorize]
    public class GameController : ControllerBase
    {
        private readonly ICreateGameHandler _createGameHandler; // ✅ Použití interface
        private readonly ILogger<GameController> _logger;
        private readonly IGameService _gameService;
        private readonly IWaitingRoomListService _waitingRoomService;

        public GameController(ICreateGameHandler createGameHandler, IGameService gameService, IWaitingRoomListService waitingRoomService, ILogger<GameController> logger)
        {
            _createGameHandler = createGameHandler;
            _logger = logger;
            _gameService = gameService;
            _waitingRoomService = waitingRoomService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame()
        {
            var dto = new CreateGameDto { HostUserId = 1 }; // Simulace přijatých dat
            var game = await _createGameHandler.HandleAsync(dto);

            if (game == null)
                return BadRequest("Game creation failed.");

            var waitingRoom = await _waitingRoomService.CreateWaitingRoomAsync(game.Id);

            return Ok(new
            {
                GameId = game.Id,
                WaitingRoomId = waitingRoom?.Id
            });
        }
    }
}