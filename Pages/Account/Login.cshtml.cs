using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BloodDonationTracker.Models;

namespace BloodDonationTracker.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin")) return RedirectToAction("Index", "Admin");
                if (roles.Contains("Hospital")) return RedirectToAction("Index", "Hospital");
                if (roles.Contains("Donor")) return RedirectToAction("Index", "Donor");
            }

            return RedirectToPage("/Index");
        }
    }
}

