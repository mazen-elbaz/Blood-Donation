using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BloodDonationTracker.Models;
using BloodDonationTracker.Data;

namespace BloodDonationTracker.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(string firstName, string lastName, string email, string password, string city, string role)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                City = city,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _userManager.AddToRoleAsync(user, role);
            await _signInManager.SignInAsync(user, isPersistent: false);

            if (role == "Donor")
            {
                _context.Donors.Add(new BloodDonationTracker.Models.Donor
                {
                    UserId = user.Id,
                    BloodType = "O+",
                    IsAvailable = true,
                    LastDonationDate = DateTime.Now.AddMonths(-6)
                });
            }
            else if (role == "Hospital")
            {
                _context.Hospitals.Add(new BloodDonationTracker.Models.Hospital
                {
                    UserId = user.Id,
                    HospitalName = $"{firstName} {lastName} Hospital",
                    Address = "Address to be updated",
                    PhoneNumber = "Phone to be updated"
                });
            }

            await _context.SaveChangesAsync();

            if (role == "Admin") return RedirectToAction("Index", "Admin");
            if (role == "Hospital") return RedirectToAction("Index", "Hospital");
            if (role == "Donor") return RedirectToAction("Index", "Donor");

            return RedirectToPage("/Index");
        }
    }
}

