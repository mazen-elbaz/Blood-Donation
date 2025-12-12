using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BloodDonationTracker.Data;
using BloodDonationTracker.Models;

namespace BloodDonationTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var stats = new
            {
                TotalDonors = await _context.Donors.CountAsync(),
                TotalHospitals = await _context.Hospitals.CountAsync(),
                OpenRequests = await _context.BloodRequests.CountAsync(r => r.Status == "Open"),
                CompletedDonations = await _context.Donations.CountAsync(d => d.Status == "Completed")
            };

            // Check if user is authenticated
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentUser;
            ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
            ViewBag.Stats = stats;
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}

