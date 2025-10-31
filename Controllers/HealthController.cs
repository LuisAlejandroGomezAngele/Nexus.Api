using Microsoft.AspNetCore.Mvc;

namespace Nexus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API funcionando");
        }
    }
}
