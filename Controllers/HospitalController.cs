using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BloodDonationTracker.Data;
using BloodDonationTracker.Models;

namespace BloodDonationTracker.Controllers
{
    [Authorize(Roles = "Hospital")]
    public class HospitalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HospitalController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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

            var hospital = await _context.Hospitals
                .Include(h => h.User)
                .FirstOrDefaultAsync(h => h.UserId == user.Id);

            if (hospital == null)
            {
                return RedirectToAction("Register", "Account");
            }

            var openRequests = await _context.BloodRequests
                .Where(br => br.HospitalId == hospital.Id && br.Status == "Open")
                .CountAsync();

            var fulfilledRequests = await _context.BloodRequests
                .Where(br => br.HospitalId == hospital.Id && br.Status == "Fulfilled")
                .CountAsync();

            var recentRequests = await _context.BloodRequests
                .Where(br => br.HospitalId == hospital.Id)
                .OrderByDescending(br => br.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.Hospital = hospital;
            ViewBag.OpenRequests = openRequests;
            ViewBag.FulfilledRequests = fulfilledRequests;
            ViewBag.RecentRequests = recentRequests;

            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var hospital = await _context.Hospitals
                .Include(h => h.User)
                .FirstOrDefaultAsync(h => h.UserId == user.Id);

            return View(hospital);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(Hospital model)
        {
            var user = await _userManager.GetUserAsync(User);
            var hospital = await _context.Hospitals
                .FirstOrDefaultAsync(h => h.UserId == user.Id);

            if (hospital != null)
            {
                hospital.HospitalName = model.HospitalName;
                hospital.Address = model.Address;
                hospital.PhoneNumber = model.PhoneNumber;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }

            return RedirectToAction("Profile");
        }

        public IActionResult CreateRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(BloodRequest model)
        {
            var user = await _userManager.GetUserAsync(User);
            var hospital = await _context.Hospitals
                .FirstOrDefaultAsync(h => h.UserId == user.Id);

            if (hospital != null)
            {
                model.HospitalId = hospital.Id;
                model.RequestDate = DateTime.Now;
                model.Status = "Open";

                _context.BloodRequests.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Blood request created successfully!";
                return RedirectToAction("MyRequests");
            }

            return View(model);
        }

        public async Task<IActionResult> MyRequests()
        {
            var user = await _userManager.GetUserAsync(User);
            var hospital = await _context.Hospitals
                .FirstOrDefaultAsync(h => h.UserId == user.Id);

            var requests = await _context.BloodRequests
                .Where(br => br.HospitalId == hospital.Id)
                .OrderByDescending(br => br.CreatedAt)
                .ToListAsync();

            return View(requests);
        }

        public async Task<IActionResult> RequestDetails(int id)
        {
            var request = await _context.BloodRequests
                .Include(br => br.Hospital)
                .Include(br => br.Donations)
                .ThenInclude(d => d.Donor)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(br => br.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRequestStatus(int requestId, string status)
        {
            var request = await _context.BloodRequests
                .FirstOrDefaultAsync(br => br.Id == requestId);

            if (request != null)
            {
                request.Status = status;
                if (status == "Fulfilled")
                {
                    request.FulfilledDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Request status updated successfully!";
            }

            return RedirectToAction("MyRequests");
        }
    }
}
