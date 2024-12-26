using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CabRosterApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } // User's Name
        //public string Role { get; set; } // Role (e.g., Admin, Employee)
        
        public bool IsApproved { get; set; } = false; // Default value is false

        public bool IsRejected { get; set; } = false; // Rejection status, default is false

        [Phone]
        public string MobileNumber { get; set; }

    }
}
