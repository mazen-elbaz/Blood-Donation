using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodDonationTracker.Models
{
    public class Donation
    {
        public int Id { get; set; }

        [Required]
        public int DonorId { get; set; }

        [ForeignKey("DonorId")]
        public Donor Donor { get; set; } = null!;

        [Required]
        public int BloodRequestId { get; set; }

        [ForeignKey("BloodRequestId")]
        public BloodRequest BloodRequest { get; set; } = null!;

        [Required]
        public DateTime DonationDate { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? Notes { get; set; }
    }
}

