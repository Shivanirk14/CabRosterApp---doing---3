using CabRosterApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CalendarController : Controller
{
    private readonly CabRosterAppDbContext _context;

    public CalendarController(CabRosterAppDbContext context)
    {
        _context = context;
    }

    // GET: /api/Calendar/available-dates
    [HttpGet("available-dates")]
    public IActionResult GetAvailableDates(int weeks = 4)
    {
        if (weeks <= 0)
        {
            return BadRequest(new { Error = "Weeks parameter must be greater than 0." });
        }

        var today = DateTime.Now.Date;
        var availableDates = new List<DateTime>();

        for (var i = 0; i < weeks * 7; i++)
        {
            var date = today.AddDays(i);
            // Exclude weekends (Saturday & Sunday) and allow weekdays for booking
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                availableDates.Add(date);
            }
        }

        // Logging available dates in console for debugging
        foreach (var date in availableDates)
        {
            Console.WriteLine($"Available date: {date.ToString("yyyy-MM-dd")}");
        }

        return Ok(availableDates.Select(d => d.ToString("yyyy-MM-dd")));
    }

    [HttpPost("book-cab")]
    public async Task<IActionResult> BookCab([FromBody] DateRangeRequest request)
    {
        // Log received request data
        Console.WriteLine($"Received booking request: {JsonConvert.SerializeObject(request)}");

        // Validate the request model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var cabBooking = new CabBooking
            {
                UserId = request.UserId,
                ShiftId = request.ShiftId,
                NodalPointId = request.NodalPointId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                BookingDate = DateTime.Now,
                Status = "Booked"
            };

            // Save the booking to the database
            _context.CabBookings.Add(cabBooking);
            await _context.SaveChangesAsync();

            // Log successful booking
            Console.WriteLine($"Cab booked successfully: {cabBooking}");

            return Ok(new { Message = "Cab successfully booked!", BookingDetails = cabBooking });
        }
        catch (Exception ex)
        {
            // Log exception for debugging
            Console.WriteLine($"Error booking cab: {ex.Message}");
            return StatusCode(500, new { Error = $"An error occurred: {ex.Message}" });
        }
    }
}
