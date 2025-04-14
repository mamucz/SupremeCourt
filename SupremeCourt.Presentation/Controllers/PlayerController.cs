using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Application.CQRS.Players.Commands;
using SupremeCourt.Application.CQRS.WaitingRooms.Queries;
using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase
    {
        private readonly CreatePlayerHandler _createPlayerHandler;
        private readonly IMediator _mediator;

        public PlayerController(CreatePlayerHandler createPlayerHandler, IMediator mediator)
        {
            _createPlayerHandler = createPlayerHandler;
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves the actual state of a player (online, in waiting room, in game).
        /// </summary>
        /// <param name="id">The ID of the player (userId).</param>
        /// <returns>200 OK with player's current state.</returns>
        [HttpGet("playeractualstate/{id}")]
        [ProducesResponseType(typeof(ActualPlayerStateDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPlayerActualState(int id)
        {
            var result = await _mediator.Send(new GetActualPlayerStateQuery(id));
            return Ok(result);
        }
    }
    
}