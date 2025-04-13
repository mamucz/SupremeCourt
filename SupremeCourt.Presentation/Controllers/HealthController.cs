using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SupremeCourt.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogDebug("Health check requested at {Time}", DateTime.UtcNow);
            return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
        }
    }
}