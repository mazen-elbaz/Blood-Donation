using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodDonationTracker.Models
{
    public class BloodRequest
    {
        public int Id { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [ForeignKey("HospitalId")]
        public Hospital Hospital { get; set; } = null!;

        [Required]
        [StringLength(5)]
        public string BloodType { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        [StringLength(20)]
        public string Urgency { get; set; } = "Normal"; // Low, Normal, High, Critical

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Open"; // Open, Fulfilled, Cancelled

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public DateTime? FulfilledDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    }
}

