using CabRosterApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CabRosterApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CabRosterAppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(CabRosterAppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetUsersStatus()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            var userStatus = new
            {
                user.Name,
                user.Email,
                user.IsApproved,
                user.IsRejected,
                user.MobileNumber
            };

            return Ok(new { Success = true, Data = userStatus });
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            // Check if the user with the same email already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { Success = false, Error = "An account with this email already exists." });
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                MobileNumber = model.MobileNumber,
                IsApproved = false,
                IsRejected = false,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Success = true, Message = "User registered successfully. Awaiting approval." });
            }

            var errorMessages = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { Success = false, Errors = errorMessages });
        }
    }
}
