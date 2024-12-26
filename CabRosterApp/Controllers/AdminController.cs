using CabRosterApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CabRosterApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CabRosterAppDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, CabRosterAppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: View all pending bookings
        [HttpGet("cab-bookings/pending")]
        public IActionResult GetPendingCabBookings()
        {
            var pendingBookings = _context.CabBookings
                .Where(b => b.Status == "Pending")
                .Select(b => new
                {
                    b.Id,
                    b.BookingDate,
                    Shift = b.Shift.ShiftTime,
                    EmployeeName = b.User.Name
                })
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            if (!pendingBookings.Any())
            {
                return NotFound(new { Message = "No pending cab bookings found." });
            }

            return Ok(pendingBookings);
        }

        // PUT: Approve or Reject a booking
        [HttpPut("cab-bookings/{id}/status")]
        public IActionResult UpdateCabBookingStatus(int id, [FromBody] string status)
        {
            if (status != "Approved" && status != "Rejected")
            {
                return BadRequest(new { Error = "Invalid status. Use 'Approved' or 'Rejected'." });
            }

            var booking = _context.CabBookings.Find(id);
            if (booking == null)
            {
                return NotFound(new { Error = "Booking not found." });
            }

            booking.Status = status;
            _context.SaveChanges();

            return Ok(new { Message = $"Booking status updated to {status}.", BookingId = id });
        }

        // GET: List all employees
        [HttpGet("employees")]
        public IActionResult GetAllEmployees()
        {
            var employees = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.MobileNumber,
                    //Area = u.Area ?? "Not specified"
                })
                .ToList();

            if (!employees.Any())
            {
                return NotFound(new { Message = "No employees found." });
            }

            return Ok(employees);
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            var totalUsers = _context.Users.Count();
            var pendingUsers = GetPendingUsers().Count();
            var approvedUsers = GetApprovedUsers().Count();
            var rejectedUsers = GetRejectedUsers().Count();

            return Ok(new
            {
                TotalUsers = totalUsers,
                PendingUsers = pendingUsers,
                ApprovedUsers = approvedUsers,
                RejectedUsers = rejectedUsers
            });
        }

        // GET: Retrieve all users categorized by approval status
        [HttpGet("get-all-users")]
        public IActionResult GetAllUsers()
        {
            var response = new
            {
                PendingUsers = GetPendingUsers().ToList(),
                ApprovedUsers = GetApprovedUsers().ToList(),
                RejectedUsers = GetRejectedUsers().ToList()
            };

            return Ok(response);
        }

        // POST: Approve a user
        [HttpPost("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Error = "User not found." });
            }

            if (user.IsApproved)
            {
                return BadRequest(new { Error = "User is already approved." });
            }

            user.IsApproved = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { Error = "Failed to approve the user." });
            }

            return Ok(new { Message = "User approved successfully." });
        }

        // POST: Reject a user
        [HttpPost("reject-user/{userId}")]
        public async Task<IActionResult> RejectUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound(new { Error = "User not found." });

            user.IsRejected = true;
            user.IsApproved = false;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(new { Error = "Failed to reject the user." });

            return Ok(new { Message = "User rejected successfully." });
        }

        // Private methods for user categorization
        private IQueryable<ApplicationUser> GetPendingUsers()
        {
            return _userManager.Users.Where(u => !u.IsApproved && !u.IsRejected);
        }

        private IQueryable<ApplicationUser> GetApprovedUsers()
        {
            return _userManager.Users.Where(u => u.IsApproved);
        }

        private IQueryable<ApplicationUser> GetRejectedUsers()
        {
            return _userManager.Users.Where(u => u.IsRejected);
        }
    }
}
