using CabRosterApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CabRosterApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodalPointController : ControllerBase
    {
        private readonly CabRosterAppDbContext _context;

        public NodalPointController(CabRosterAppDbContext context)
        {
            _context = context;
        }

        // POST: /api/NodalPoint/add-nodal-point
        [HttpPost("add-nodal-point")]
        public async Task<IActionResult> AddNodalPoint([FromBody] NodalPoint nodalPoint)
        {
            if (nodalPoint == null || string.IsNullOrWhiteSpace(nodalPoint.LocationName))
            {
                return BadRequest("Invalid nodal point: Location name cannot be empty.");
            }

            // Check if a nodal point with the same location name already exists
            var existingNodalPoint = await _context.NodalPoints
                .Where(np => np.LocationName == nodalPoint.LocationName)
                .FirstOrDefaultAsync();

            if (existingNodalPoint != null)
            {
                return BadRequest("A nodal point with this location already exists.");
            }

            // Add new nodal point
            _context.NodalPoints.Add(nodalPoint);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Nodal point added successfully." });
        }

        // GET: /api/NodalPoint/get-nodal-points
        [HttpGet("get-nodal-points")]
        public async Task<IActionResult> GetNodalPoints()
        {
            var nodalPoints = await _context.NodalPoints.ToListAsync();
            return Ok(nodalPoints);
        }
    }
}
