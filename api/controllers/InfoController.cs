using Microsoft.AspNetCore.Mvc;

namespace Abschlussprojekt.Controllers
{
    [ApiController]
    [Route("info")]
    public class InfoController : ControllerBase
    {
        // Eindeutige ID pro Instanz
        private static readonly string InstanceId = Guid.NewGuid().ToString("N");

        [HttpGet]
        public IActionResult Get()
        {
            // Port, auf dem diese Instanz läuft
            var port = HttpContext.Request.Host.Port;

            return Ok(new
            {
                instanceId = InstanceId,
                port = port,
                timestampUtc = DateTime.UtcNow
            });
        }
    }
}
