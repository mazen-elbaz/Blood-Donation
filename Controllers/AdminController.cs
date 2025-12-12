using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BloodDonationTracker.Data;
using BloodDonationTracker.Models;

namespace BloodDonationTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalDonors = await _context.Donors.CountAsync(),
                TotalHospitals = await _context.Hospitals.CountAsync(),
                TotalRequests = await _context.BloodRequests.CountAsync(),
                OpenRequests = await _context.BloodRequests.CountAsync(r => r.Status == "Open"),
                CompletedDonations = await _context.Donations.CountAsync(d => d.Status == "Completed")
            };

            var recentDonations = await _context.Donations
                .Include(d => d.Donor)
                .ThenInclude(d => d.User)
                .Include(d => d.BloodRequest)
                .ThenInclude(br => br.Hospital)
                .ThenInclude(h => h.User)
                .OrderByDescending(d => d.CreatedAt)
                .Take(10)
                .ToListAsync();

            ViewBag.Stats = stats;
            ViewBag.RecentDonations = recentDonations;

            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();

            var usersWithRoles = new List<dynamic>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new
                {
                    User = user,
                    Roles = roles
                });
            }

            return View(usersWithRoles);
        }

        public async Task<IActionResult> Donors()
        {
            var donors = await _context.Donors
                .Include(d => d.User)
                .Include(d => d.Donations)
                .ToListAsync();

            return View(donors);
        }

        public async Task<IActionResult> Hospitals()
        {
            var hospitals = await _context.Hospitals
                .Include(h => h.User)
                .Include(h => h.BloodRequests)
                .ToListAsync();

            return View(hospitals);
        }

        public async Task<IActionResult> BloodRequests()
        {
            var requests = await _context.BloodRequests
                .Include(br => br.Hospital)
                .ThenInclude(h => h.User)
                .Include(br => br.Donations)
                .ThenInclude(d => d.Donor)
                .ThenInclude(d => d.User)
                .OrderByDescending(br => br.CreatedAt)
                .ToListAsync();

            return View(requests);
        }

        public async Task<IActionResult> Donations()
        {
            var donations = await _context.Donations
                .Include(d => d.Donor)
                .ThenInclude(d => d.User)
                .Include(d => d.BloodRequest)
                .ThenInclude(br => br.Hospital)
                .ThenInclude(h => h.User)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return View(donations);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                TempData["SuccessMessage"] = "User deleted successfully!";
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDonationStatus(int donationId, string status)
        {
            var donation = await _context.Donations
                .FirstOrDefaultAsync(d => d.Id == donationId);

            if (donation != null)
            {
                donation.Status = status;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Donation status updated successfully!";
            }

            return RedirectToAction("Donations");
        }
    }
}
