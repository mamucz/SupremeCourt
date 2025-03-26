using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Presentation.Controllers
{
    [ApiController]
    [Route("api/waitingroom")]
    [Authorize]
    public class WaitingRoomController : ControllerBase
    {
        private readonly IWaitingRoomService _waitingRoomService;
        private readonly IGameService _gameService;

        public WaitingRoomController(IWaitingRoomService waitingRoomService, IGameService gameService)
        {
            _waitingRoomService = waitingRoomService;
            _gameService = gameService;
        }

        /// <summary>
        /// Vytvoří novou hru a waiting room.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateGameAndWaitingRoom([FromBody] CreateGameDto dto)
        {
            var game = await _gameService.CreateGameAsync();
            if (game == null)
                return BadRequest("Game creation failed.");

            var waitingRoom = await _waitingRoomService.CreateWaitingRoomAsync(game.Id);
            if (waitingRoom == null)
                return BadRequest("Waiting room creation failed.");

            return Ok(new
            {
                GameId = game.Id,
                WaitingRoomId = waitingRoom.Id
            });
        }

        /// <summary>
        /// Připojí hráče do waiting room.
        /// </summary>
        [HttpPost("join")]
        public async Task<IActionResult> JoinWaitingRoom([FromBody] JoinGameRequest request)
        {
            var result = await _waitingRoomService.JoinWaitingRoomAsync(request.GameId, request.PlayerId);
            if (!result) return BadRequest("Failed to join the game.");

            return Ok("Joined successfully.");
        }
    }
}