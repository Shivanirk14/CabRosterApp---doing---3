using System;
using System.ComponentModel.DataAnnotations;

namespace CabRosterApp.Models
{
    public class DateRangeRequest
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }  // Change to string if UserId is a GUID or alphanumeric

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Shift ID is required.")]
        public int ShiftId { get; set; }
        [Required(ErrorMessage = "Nodal point ID is required.")]
        public int NodalPointId { get; set; }
    }
}
