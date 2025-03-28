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

        // <summary>
        /// Vytvoří novou waiting room pro hráče.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateWaitingRoom([FromBody] CreateWaitingRoomRequest request)
        {
            if (request.PlayerId <= 0)
                return BadRequest("Neplatné ID hráče.");

            var waitingRoom = await _waitingRoomService.CreateWaitingRoomAsync(request.PlayerId);
            if (waitingRoom == null)
                return BadRequest("Waiting room creation failed.");

            return Ok(new
            {
                WaitingRoomId = waitingRoom.Id,
                CreatedAt = waitingRoom.CreatedAt,
                CreatedByPlayerId = waitingRoom.CreatedByPlayerId
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

        [HttpGet("waitingrooms")]
        public async Task<IActionResult> GetWaitingRooms()
        {
            var rooms = await _waitingRoomService.GetWaitingRoomSummariesAsync();
            return Ok(rooms);
        }
    }
}