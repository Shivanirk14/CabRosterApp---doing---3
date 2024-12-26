namespace CabRosterApp.Models
{
    public class CabBookingRequest
    {
        public string UserId { get; set; }  // Changed back to string
        public List<DateTime> BookingDates { get; set; }
        public int ShiftId { get; set; }
        public int NodalPointId { get; set; }
    }
}