using Microsoft.AspNetCore.Mvc;

namespace InventoryManagerApi.Controllers
{
    [Route("api/health")]
    public class HealthController : Controller
    {
        [HttpGet()]
        public IActionResult CheckHealth()
        {
            return Ok("API funcionando...");
        }
    }
}
