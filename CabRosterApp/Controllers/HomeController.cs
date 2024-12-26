using Microsoft.AspNetCore.Mvc;

namespace CabRosterApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // GET: api/Home
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Message = "Welcome to CabRosterApp",
                Version = "1.0.0", // Versioning may be done dynamically in future
                DocumentationUrl = "https://your-docs-link.com", // Placeholder for documentation link
                Timestamp = DateTime.UtcNow // UTC timestamp for better tracking
            });
        }
    }
}
