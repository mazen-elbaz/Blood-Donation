using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodDonationTracker.Models
{
    public class Donor
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        [StringLength(5)]
        public string BloodType { get; set; } = string.Empty;

        [Required]
        public bool IsAvailable { get; set; } = true;

        public DateTime LastDonationDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    }
}

