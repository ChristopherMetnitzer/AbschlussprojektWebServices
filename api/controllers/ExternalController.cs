using Microsoft.AspNetCore.Mvc;
using Polly.Retry;
using Abschlussprojekt.Services;

namespace Abschlussprojekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalController : ControllerBase
    {
        private readonly ExternalUnstableService _externalService;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ILogger<ExternalController> _logger;

        public ExternalController(ExternalUnstableService externalService, AsyncRetryPolicy retryPolicy,
            ILogger<ExternalController> logger)
        {
            _externalService = externalService;
            _retryPolicy = retryPolicy;
            _logger = logger;
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetExternalData()
        {
            int attempt = 0;
            try
            {
                var result = await _retryPolicy.ExecuteAsync(async () =>
                {
                    attempt++;
                    _logger.LogInformation("Attempt {Attempt} to call external service.", attempt);
                    return await _externalService.CallAsync();
                });

                return Ok(new { Data = result, Attempts = attempt });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "All attempts to call external service failed after {Attempts} tries.", attempt);
                return StatusCode(503, "External service is unavailable. Please try again later.");
            }
        }
}
}

