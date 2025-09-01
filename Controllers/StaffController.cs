using EMiningLicense.Data;
using EMiningLicense.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMiningLicense.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        public StaffController(AppDbContext context,UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var newApps = await _context.LicenseApplications
       .Where(a => a.Status == "Pending")
       .Select(a => new LicenseApplicationVm
       {
           Id = a.Id,
           ApplicantName = a.ContactPerson,
           CompanyName = a.CompanyName,
           LicenseType = a.MiningType,
           SubmittedAt = a.SubmittedAt,
           Status = a.Status,
           Latitude = a.Latitude,
           Longitude = a.Longitude,
           DocumentPath = a.DocumentPath
       })
       .ToListAsync();
            // fetch "assigned applications"
            var assignedApps = await _context.LicenseApplications
                .Where(a => a.Status == "Assigned")
                .Select(a => new AssignedApplicationVm
                {
                    Id = a.Id,
                    ApplicantName = a.ContactPerson,
                    CompanyName = a.CompanyName,
                    LicenseType = a.MiningType,
                    SubmittedAt = a.SubmittedAt,
                    Status = a.Status,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    DocumentPath = a.DocumentPath
                })
                .ToListAsync();

            // Fetch assigned apps for the current staff
            var user = await _userManager.GetUserAsync(User);

            var vm = new StaffDashboardVm
            {
                Notifications = new List<string>
        {
            "📢 Welcome to your staff dashboard!"
        },
                NewApplications = newApps,
                AssignedApplications = assignedApps
            };
       

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var vm = new EditProfileVm
            {
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Organization = user.Organization
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(EditProfileVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            user.FullName = vm.FullName;
            user.Email = vm.Email;
            user.UserName = vm.Email; // keep email as username
            user.PhoneNumber = vm.Phone;
            user.Organization = vm.Organization;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["msg"] = "✅ Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            foreach (var e in result.Errors)
                ModelState.AddModelError("", e.Description);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVm vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["pwdError"] = "⚠️ Please fill all fields correctly.";
                return RedirectToAction("Profile");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var result = await _userManager.ChangePasswordAsync(user, vm.CurrentPassword, vm.NewPassword);

            if (result.Succeeded)
            {
                TempData["pwdMsg"] = "✅ Password updated successfully!";
            }
            else
            {
                TempData["pwdError"] = string.Join(" | ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Profile");
        }
    }
}
