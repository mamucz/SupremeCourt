using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Application.Players.Commands;

namespace SupremeCourt.Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase
    {
        private readonly CreatePlayerHandler _createPlayerHandler;

        public PlayerController(CreatePlayerHandler createPlayerHandler)
        {
            _createPlayerHandler = createPlayerHandler;
        }

    }
}