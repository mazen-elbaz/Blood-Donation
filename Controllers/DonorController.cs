using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BloodDonationTracker.Data;
using BloodDonationTracker.Models;

namespace BloodDonationTracker.Controllers
{
    [Authorize(Roles = "Donor")]
    public class DonorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DonorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var donor = await _context.Donors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (donor == null)
            {
                return RedirectToAction("Register", "Account");
            }

            var recentDonations = await _context.Donations
                .Include(d => d.BloodRequest)
                .ThenInclude(br => br.Hospital)
                .ThenInclude(h => h.User)
                .Where(d => d.DonorId == donor.Id)
                .OrderByDescending(d => d.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.Donor = donor;
            ViewBag.RecentDonations = recentDonations ?? new List<Donation>();

            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var donor = await _context.Donors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            return View(donor);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(Donor model)
        {
            var user = await _userManager.GetUserAsync(User);
            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (donor != null)
            {
                donor.BloodType = model.BloodType;
                donor.IsAvailable = model.IsAvailable;
                donor.LastDonationDate = model.LastDonationDate;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }

            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> ViewRequests()
        {
            var user = await _userManager.GetUserAsync(User);
            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            var requests = await _context.BloodRequests
                .Include(br => br.Hospital)
                .ThenInclude(h => h.User)
                .Where(br => br.Status == "Open" && br.BloodType == donor.BloodType)
                .OrderByDescending(br => br.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            var user = await _userManager.GetUserAsync(User);
            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            var request = await _context.BloodRequests
                .FirstOrDefaultAsync(br => br.Id == requestId);

            if (donor != null && request != null && donor.IsAvailable)
            {
                var donation = new Donation
                {
                    DonorId = donor.Id,
                    BloodRequestId = request.Id,
                    DonationDate = DateTime.Now.AddDays(1), // Schedule for tomorrow
                    Quantity = Math.Min(request.Quantity, 1), // Donate 1 unit
                    Status = "Pending"
                };

                _context.Donations.Add(donation);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "You have accepted the blood request!";
            }

            return RedirectToAction("ViewRequests");
        }

        public async Task<IActionResult> MyDonations()
        {
            var user = await _userManager.GetUserAsync(User);
            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            var donations = await _context.Donations
                .Include(d => d.BloodRequest)
                .ThenInclude(br => br.Hospital)
                .ThenInclude(h => h.User)
                .Where(d => d.DonorId == donor.Id)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return View(donations);
        }
    }
}
