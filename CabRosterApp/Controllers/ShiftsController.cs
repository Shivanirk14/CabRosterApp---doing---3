using CabRosterApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CabRosterApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftsController : ControllerBase
    {
        private readonly CabRosterAppDbContext _context;

        public ShiftsController(CabRosterAppDbContext context)
        {
            _context = context;
        }

        // GET: api/Shifts/list
        [HttpGet("list")]
        public IActionResult GetShifts(int page = 1, int pageSize = 10)
        {
            try
            {
                var shifts = _context.Shifts
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new
                    {
                        id = s.Id,
                        shiftTime = s.ShiftTime
                    })
                    .ToList();

                if (!shifts.Any())
                {
                    return NotFound(new { Message = "No shifts found." });
                }

                return Ok(shifts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while fetching shifts.", Details = ex.Message });
            }
        }
    }
}
