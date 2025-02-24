using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Application.Games.Commands;
using SupremeCourt.Domain.Entities;
using Microsoft.Extensions.Logging;
using SupremeCourt.Application.Services;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Application.DTOs;

namespace SupremeCourt.Presentation.Controllers
{
    [Route("api/game")]
    [ApiController]
    [Authorize] // Vyžaduje autorizaci pomocí JWT
    public class GameController : ControllerBase
    {
        private readonly CreateGameHandler _createGameHandler;
        private readonly ILogger<GameController> _logger;
        private readonly IGameService _gameService;
        private readonly IWaitingRoomService _waitingRoomService;

        public GameController(CreateGameHandler createGameHandler, IGameService gameService, IWaitingRoomService waitingRoomService, ILogger<GameController> logger)
        {
            _createGameHandler = createGameHandler;
            _logger = logger;
            _gameService = gameService;
            _waitingRoomService = waitingRoomService;

        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame()
        {
            var game = await _gameService.CreateGameAsync();
            if (game == null) return BadRequest("Game creation failed.");

            var waitingRoom = await _waitingRoomService.CreateWaitingRoomAsync(game.Id);

            return Ok(new
            {
                GameId = game.Id,
                WaitingRoomId = waitingRoom?.Id
            });
        }


        [HttpPost("join")]
        public async Task<IActionResult> JoinGame([FromBody] JoinGameRequest request)
        {
            var result = await _waitingRoomService.JoinWaitingRoomAsync(request.GameId, request.PlayerId);
            if (!result) return BadRequest("Failed to join the game.");

            return Ok("Joined successfully.");
        }

    }



    public class CreateGameRequest
    {
        public int HostUserId { get; set; } // ID uživatele, který zakládá hru
    }
}