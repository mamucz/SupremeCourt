using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Application.CQRS.WaitingRooms.Commands;
using SupremeCourt.Application.CQRS.WaitingRooms.Queries;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Presentation.Controllers
{
    [ApiController]
    [Route("api/waitingroom")]
    [Authorize]
    public class WaitingRoomController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WaitingRoomController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new waiting room for a player.
        /// </summary>
        /// <param name="request">Contains the player ID who is creating the room.</param>
        /// <returns>
        /// 200 OK with created room info.  
        /// 400 BadRequest if creation failed.
        /// </returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWaitingRoom([FromBody] CreateWaitingRoomRequest request)
        {
            var result = await _mediator.Send(new CreateWaitingRoomCommand(request.PlayerId));
            if (result == null)
                return BadRequest(new { message = "Waiting room creation failed." });

            return Ok(result);
        }

        /// <summary>
        /// Adds a player to an existing waiting room.
        /// </summary>
        /// <param name="request">Contains the waiting room ID and the player ID.</param>
        /// <returns>
        /// 200 OK if joined successfully.  
        /// 400 BadRequest if join fails (e.g., already in another room, room full).
        /// </returns>
        [HttpPost("join")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> JoinWaitingRoom([FromBody] JoinGameRequest request)
        {
            var result = await _mediator.Send(new JoinWaitingRoomCommand(request.WaitingRoomId, request.Player));
            if (!result)
                return BadRequest(new { message = @"Unable to join {PlayerId} the waiting room.", request.Player });

            return Ok(new { message = "Joined successfully." });
        }

        /// <summary>
        /// Retrieves a list of all available waiting rooms with less than 5 players.
        /// </summary>
        /// <returns>200 OK with the list of waiting rooms.</returns>
        [HttpGet("waitingrooms")]
        [ProducesResponseType(typeof(IEnumerable<WaitingRoomDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWaitingRooms()
        {
            var result = await _mediator.Send(new GetWaitingRoomsQuery());
            return Ok(result);
        }

        /// <summary>
        /// Gets the details of a specific waiting room by ID.
        /// </summary>
        /// <param name="id">ID of the waiting room</param>
        /// <returns>200 OK with room details or 404 NotFound</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WaitingRoomDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWaitingRoom(Guid id)
        {
            var result = await _mediator.Send(new GetWaitingRoomByIdQuery(id));
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        /// <summary>
        /// Player leaves a waiting room.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="playerId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("{id}/leave/{playerId}")]
        public async Task<IActionResult> LeaveRoom(Guid id, int playerId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new LeaveWaitingRoomCommand(id,playerId), cancellationToken);
            if (!result)
                return BadRequest("Nelze opustit místnost.");

            return Ok();
        }

        /// <summary>
        /// Get active player waiting room id
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="cancellationToken">WaitingRoomDto</param>
        /// <returns></returns>
        [HttpGet("active/{playerId}")]
        public async Task<IActionResult> GetPlayerActiveRoom(int playerId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetWaitingRoomByPlayerIdQuery(playerId), cancellationToken);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("{roomId}/add-ai-player")]
        public async Task<IActionResult> AddAiPlayer(Guid roomId, [FromBody] AddAiPlayersBulkDto dto)
        {
            await _mediator.Send(new AddAiPlayersBulkCommand(dto.Type, dto.idWaitingRoom));
            return NoContent();
        }

    }
}
