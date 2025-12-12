using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BloodDonationTracker.Models;

namespace BloodDonationTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Donor> Donors { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Donor>()
                .HasOne(d => d.User)
                .WithOne()
                .HasForeignKey<Donor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Hospital>()
                .HasOne(h => h.User)
                .WithOne()
                .HasForeignKey<Hospital>(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Donation>()
                .HasOne(d => d.Donor)
                .WithMany(d => d.Donations)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Donation>()
                .HasOne(d => d.BloodRequest)
                .WithMany(br => br.Donations)
                .HasForeignKey(d => d.BloodRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BloodRequest>()
                .HasOne(br => br.Hospital)
                .WithMany(h => h.BloodRequests)
                .HasForeignKey(br => br.HospitalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

