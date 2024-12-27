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

            if (bookingRequest.StartDate == DateTime.MinValue || bookingRequest.EndDate == DateTime.MinValue)
            {
                return BadRequest("StartDate and EndDate must be provided.");
            }

            try
            {
                foreach (var date in bookingRequest.BookingDates)
                {
                    // Check for conflicting bookings on the same date for the same shift and nodal point.
                    var existingBooking = await _context.CabBookings.AnyAsync(cb =>
                        cb.UserId == bookingRequest.UserId &&
                        cb.ShiftId == bookingRequest.ShiftId &&
                        cb.NodalPointId == bookingRequest.NodalPointId &&
                        cb.BookingDate == date);

                    if (existingBooking)
                    {
                        return Conflict(new { Error = $"A booking already exists for {date.ToShortDateString()}." });
                    }

                    // Create a new CabBooking object with the provided StartDate and EndDate
                    var newBooking = new CabBooking
                    {
                        UserId = bookingRequest.UserId,
                        ShiftId = bookingRequest.ShiftId,
                        BookingDate = date,
                        Status = "Booked",
                        NodalPointId = bookingRequest.NodalPointId,
                        StartDate = bookingRequest.StartDate, // Set the StartDate
                        EndDate = bookingRequest.EndDate      // Set the EndDate
                    };

                    _context.CabBookings.Add(newBooking);
                }

                await _context.SaveChangesAsync();

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
                    .Include(b => b.NodalPoint)  // Ensure this is included for nodal point data
                    .Select(b => new
                    {
                        b.Id,
                        StartDate = b.StartDate,   // Include StartDate
                        EndDate = b.EndDate,       // Include EndDate
                        Shift = b.Shift.ShiftTime,
                        b.Status,
                        User = b.User.Name,
                        NodalPoint = b.NodalPoint.LocationName // Return nodal point as needed
                    })
                    .ToListAsync();

                return Ok(bookings);  // Return list of bookings with StartDate and EndDate
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });  // Handle exceptions
            }
        }





        // GET /api/CabBooking/status/{id}
        [HttpGet("status/{id}")]
        public async Task<IActionResult> GetBookingStatus(int id)
        {
            var booking = await _context.CabBookings
                .Where(cb => cb.Id == id)
                .Select(cb => new
                {
                    cb.Id,
                    StartDate = cb.StartDate, // Show Start Date
                    EndDate = cb.EndDate,     // Show End Date
                    cb.Status,
                    ShiftTime = cb.Shift.ShiftTime,
                    NodalPoint = cb.NodalPoint.LocationName,  // Assuming NodalPoint has a Name property
                    UserName = cb.User.Name
                })
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                return NotFound(new { Error = "Booking not found." });
            }

            return Ok(booking);  // This will include the StartDate and EndDate
        }




        // POST /api/CabBooking/book-date-range
        [HttpPost("book-date-range")]
        public async Task<IActionResult> BookDateRange([FromBody] DateRangeRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid booking request.");
            }

            if (request.StartDate >= request.EndDate)
            {
                return BadRequest("The start date must be earlier than the end date.");
            }

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

            var booking = new CabBooking
            {
                UserId = request.UserId,
                ShiftId = request.ShiftId,
                BookingDate = request.StartDate,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = "Booked",
                NodalPointId = request.NodalPointId
            };

            _context.CabBookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Cab booking confirmed", bookingId = booking.Id });
        }
    }
}
