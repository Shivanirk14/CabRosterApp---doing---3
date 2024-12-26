using System;
using System.ComponentModel.DataAnnotations;

namespace CabRosterApp.Models
{
    public class CabBooking
    {
        [Required]
        public string UserId { get; set; }   // UserId as string if necessary

        public int ShiftId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public int NodalPointId { get; set; } // Foreign key for NodalPoint

        // Navigation properties
        public ApplicationUser User { get; set; }  // Navigation property to User
        public Shift Shift { get; set; }           // Navigation to Shift
        public NodalPoint NodalPoint { get; set; } // Navigation to NodalPoint

        // Optional, only keep if needed for business logic
        public DateTime StartDate { get; set; }  // Start Date
        public DateTime EndDate { get; set; }    // End Date

        // Assuming it's an integer primary key
        public int Id { get; set; }
    }
}
