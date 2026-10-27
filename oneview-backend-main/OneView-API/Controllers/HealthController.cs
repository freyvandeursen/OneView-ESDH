using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interface;
using Swashbuckle.AspNetCore.Annotations;
using DTO.Utility;

namespace OneView_API.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class HealthController : BaseApiController {

        private readonly IHealthService _healthService;

        public HealthController(IHealthService healthService) {
            _healthService = healthService;
        }

        [HttpGet("")]
        [SwaggerOperation(
            Summary = "Health check",
            Description = "Checks the servers health, with detail about the database."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "The database is reachable", typeof(HealthResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "The database is not reachable", typeof(HealthResponse))]
        public async Task<IActionResult> CheckDatabase() {
            var canConnect = await _healthService.CheckDatabaseConnectionAsync();

            var response = new HealthResponse {
                ApiStatus = "Healthy",
                DbStatus = canConnect ? "Connected" : "Disconnected",
                CheckedAtUtc = DateTime.UtcNow
            };

            response.Details["Database"] = canConnect ? "Accessible" : "Not reachable";
            
            return canConnect ? Ok(response) : Problem(StatusCodes.Status500InternalServerError, response);
        }
    }
}

