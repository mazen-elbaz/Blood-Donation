using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BloodDonationTracker.Models;
using BloodDonationTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    
                    if (roles.Contains("Admin"))
                        return RedirectToAction("Index", "Admin");
                    else if (roles.Contains("Hospital"))
                        return RedirectToAction("Index", "Hospital");
                    else if (roles.Contains("Donor"))
                        return RedirectToAction("Index", "Donor");
                }
                
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string password, string city, string role)
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
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Create role-specific records
                if (role == "Donor")
                {
                    var donor = new BloodDonationTracker.Models.Donor
                    {
                        UserId = user.Id,
                        BloodType = "O+", // Default, can be updated later
                        IsAvailable = true,
                        LastDonationDate = DateTime.Now.AddMonths(-6)
                    };
                    _context.Donors.Add(donor);
                }
                else if (role == "Hospital")
                {
                    var hospital = new BloodDonationTracker.Models.Hospital
                    {
                        UserId = user.Id,
                        HospitalName = $"{firstName} {lastName} Hospital",
                        Address = "Address to be updated",
                        PhoneNumber = "Phone to be updated"
                    };
                    _context.Hospitals.Add(hospital);
                }

                await _context.SaveChangesAsync();

                if (role == "Admin")
                    return RedirectToAction("Index", "Admin");
                else if (role == "Hospital")
                    return RedirectToAction("Index", "Hospital");
                else if (role == "Donor")
                    return RedirectToAction("Index", "Donor");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

