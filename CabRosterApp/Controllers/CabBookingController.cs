using CabRosterApp.Models;
using CabRosterApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CabRosterApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CabBookingController : ControllerBase
    {
        private readonly CabRosterAppDbContext _context;

        public CabBookingController(CabRosterAppDbContext context)
        {
            _context = context;
        }

        // POST /api/CabBooking/book
        [HttpPost("book")]
        public async Task<IActionResult> BookCab([FromBody] CabBookingRequest bookingRequest)
        {
            if (bookingRequest == null)
            {
                return BadRequest("Invalid booking request.");
            }

            if (bookingRequest.BookingDates == null || !bookingRequest.BookingDates.Any())
            {
                return BadRequest("No dates selected.");
            }

            if (bookingRequest.ShiftId <= 0 || bookingRequest.NodalPointId <= 0)
            {
                return BadRequest("Invalid shift or nodal point.");
            }

            try
            {
                foreach (var date in bookingRequest.BookingDates)
                {
                    // Check if there's already an existing booking for the same date, shift, and nodal point
                    var existingBooking = await _context.CabBookings.AnyAsync(cb =>
                        cb.UserId == bookingRequest.UserId &&
                        cb.ShiftId == bookingRequest.ShiftId &&
                        cb.NodalPointId == bookingRequest.NodalPointId &&
                        cb.BookingDate == date);

                    if (existingBooking)
                    {
                        // Return Conflict if a booking exists for the given date
                        return Conflict(new { Error = $"A booking already exists for {date.ToShortDateString()}." });
                    }

                    // Create a new booking record for the date
                    var newBooking = new CabBooking
                    {
                        UserId = bookingRequest.UserId,
                        ShiftId = bookingRequest.ShiftId,
                        BookingDate = date,
                        Status = "Booked",
                        NodalPointId = bookingRequest.NodalPointId
                    };

                    _context.CabBookings.Add(newBooking); // Add new booking to the context
                }

                await _context.SaveChangesAsync(); // Save all changes to the database

                return Ok(new { Success = true, Message = "Cab booked successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // GET /api/CabBooking/list
        [HttpGet("list")]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                var bookings = await _context.CabBookings
                    .Include(b => b.Shift)
                    .Include(b => b.User)
                    .Select(b => new
                    {
                        b.Id,
                        b.BookingDate,
                        Shift = b.Shift.ShiftTime,  // Retrieve Shift Time
                        b.Status,
                        User = b.User.Name,         // Fetch User Name
                        b.NodalPointId
                    })
                    .ToListAsync();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // PUT /api/CabBooking/update-status/{id}
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] string status)
        {
            // Ensure valid status
            if (string.IsNullOrWhiteSpace(status) || (status != "Approved" && status != "Rejected"))
            {
                return BadRequest(new { Error = "Invalid status. Use 'Approved' or 'Rejected'." });
            }

            try
            {
                var booking = await _context.CabBookings.FindAsync(id);
                if (booking == null)
                {
                    return NotFound(new { Error = "Booking not found." });
                }

                booking.Status = status;  // Update the booking status
                await _context.SaveChangesAsync();

                return Ok(new { Message = $"Booking status updated to {status}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // POST /api/CabBooking/book-date-range
        [HttpPost("book-date-range")]
        public async Task<IActionResult> BookDateRange([FromBody] DateRangeRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid booking request.");
            }

            // Validate date range
            if (request.StartDate >= request.EndDate)
            {
                return BadRequest("The start date must be earlier than the end date.");
            }

            // Check for any overlapping bookings within the given date range
            var existingBooking = await _context.CabBookings
                .AnyAsync(cb =>
                    cb.UserId == request.UserId &&
                    cb.ShiftId == request.ShiftId &&
                    cb.NodalPointId == request.NodalPointId &&
                    cb.BookingDate >= request.StartDate &&
                    cb.BookingDate <= request.EndDate);

            if (existingBooking)
            {
                return Conflict(new { Error = "There is already an existing booking within the selected date range." });
            }

            // If everything is fine, create the booking
            var booking = new CabBooking
            {
                UserId = request.UserId,  // Assuming UserId is now a string in the model
                ShiftId = request.ShiftId,
                BookingDate = request.StartDate,  // Or you may create multiple bookings if needed
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = "Booked",
                NodalPointId = request.NodalPointId
            };

            _context.CabBookings.Add(booking);  // Add the booking to the context
            await _context.SaveChangesAsync();  // Save to the database

            return Ok(new { Success = true, Message = "Cab booking confirmed", bookingId = booking.Id });
        }
    }
}
