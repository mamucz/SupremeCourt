using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Application.Players.Commands;

namespace SupremeCourt.Presentation.Controllers;

[ApiController]
[Route("api/player")]
public class PlayerController : ControllerBase
{
    private readonly CreatePlayerHandler _createPlayerHandler;

    public PlayerController(CreatePlayerHandler createPlayerHandler)
    {
        _createPlayerHandler = createPlayerHandler;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerCommand command)
    {
        var player = await _createPlayerHandler.Handle(command);
        return Ok(player);
    }
}