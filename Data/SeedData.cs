using Microsoft.AspNetCore.Identity;
using BloodDonationTracker.Models;

namespace BloodDonationTracker.Data
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Create roles if they don't exist
            string[] roles = { "Admin", "Hospital", "Donor" };
            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create Admin user
            if (await userManager.FindByEmailAsync("admin@bloodtracker.com") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@bloodtracker.com",
                    Email = "admin@bloodtracker.com",
                    FirstName = "Admin",
                    LastName = "User",
                    City = "New York",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Create Hospital user
            if (await userManager.FindByEmailAsync("hospital@citymedical.com") == null)
            {
                var hospitalUser = new ApplicationUser
                {
                    UserName = "hospital@citymedical.com",
                    Email = "hospital@citymedical.com",
                    FirstName = "City",
                    LastName = "Medical",
                    City = "New York",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(hospitalUser, "Hospital123!");
                await userManager.AddToRoleAsync(hospitalUser, "Hospital");

                // Create Hospital record
                var hospital = new BloodDonationTracker.Models.Hospital
                {
                    UserId = hospitalUser.Id,
                    HospitalName = "City Medical Center",
                    Address = "123 Medical Drive, New York, NY 10001",
                    PhoneNumber = "(555) 123-4567"
                };

                context.Hospitals.Add(hospital);
            }

            // Create Donor users
            var donor1Email = "donor1@email.com";
            if (await userManager.FindByEmailAsync(donor1Email) == null)
            {
                var donor1User = new ApplicationUser
                {
                    UserName = donor1Email,
                    Email = donor1Email,
                    FirstName = "John",
                    LastName = "Smith",
                    City = "New York",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(donor1User, "Donor123!");
                await userManager.AddToRoleAsync(donor1User, "Donor");

                // Create Donor record
                var donor1 = new BloodDonationTracker.Models.Donor
                {
                    UserId = donor1User.Id,
                    BloodType = "O+",
                    IsAvailable = true,
                    LastDonationDate = DateTime.Now.AddMonths(-3)
                };

                context.Donors.Add(donor1);
            }

            var donor2Email = "donor2@email.com";
            if (await userManager.FindByEmailAsync(donor2Email) == null)
            {
                var donor2User = new ApplicationUser
                {
                    UserName = donor2Email,
                    Email = donor2Email,
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    City = "New York",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(donor2User, "Donor123!");
                await userManager.AddToRoleAsync(donor2User, "Donor");

                // Create Donor record
                var donor2 = new BloodDonationTracker.Models.Donor
                {
                    UserId = donor2User.Id,
                    BloodType = "A+",
                    IsAvailable = true,
                    LastDonationDate = DateTime.Now.AddMonths(-2)
                };

                context.Donors.Add(donor2);
            }

            await context.SaveChangesAsync();
        }
    }
}

