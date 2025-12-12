using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BloodDonationTracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string FullName => $"{FirstName} {LastName}";
    }
}

