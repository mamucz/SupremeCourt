﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Application.CQRS.WaitingRooms.Commands;
using SupremeCourt.Application.CQRS.WaitingRooms.Queries;
using SupremeCourt.Domain.DTOs;

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
            var result = await _mediator.Send(new JoinWaitingRoomCommand(request.WaitingRoomId, request.PlayerId));
            if (!result)
                return BadRequest(new { message = "Unable to join the waiting room." });

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
    }
}
