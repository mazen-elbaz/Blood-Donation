using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BloodDonationTracker.Data;
using BloodDonationTracker.Models;

namespace BloodDonationTracker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnGet()
        {
            var stats = new
            {
                TotalDonors = await _context.Donors.CountAsync(),
                TotalHospitals = await _context.Hospitals.CountAsync(),
                OpenRequests = await _context.BloodRequests.CountAsync(r => r.Status == "Open"),
                CompletedDonations = await _context.Donations.CountAsync(d => d.Status == "Completed")
            };

            var currentUser = await _userManager.GetUserAsync(User);
            ViewData["Stats"] = stats;
            ViewData["CurrentUser"] = currentUser;
            ViewData["IsAuthenticated"] = User.Identity?.IsAuthenticated ?? false;
        }
    }
}

