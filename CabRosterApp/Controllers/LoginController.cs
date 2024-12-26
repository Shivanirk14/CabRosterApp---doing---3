using CabRosterApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CabRosterApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("LoginForm")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid input data." });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Error = "User not found." });
            }

            if (!user.IsApproved)
            {
                return Unauthorized(new { Error = "User is awaiting approval." });
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { Error = "Incorrect email or password." });
            }

            // Check for user role
            var roles = await _userManager.GetRolesAsync(user);
            bool isAdmin = roles.Contains("Admin"); // Check if the user has the 'Admin' role

            // Return user context along with role info
            return Ok(new
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsAdmin = isAdmin // Indicates if the user has 'Admin' role
            });
        }
    }
}
