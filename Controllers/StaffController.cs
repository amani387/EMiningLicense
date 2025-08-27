using EMiningLicense.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EMiningLicense.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public StaffController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new StaffDashboardVm
            {
                AssignedApplications = new List<StaffApplicationVm>
                {
                    new StaffApplicationVm { Id = 1, ApplicantName = "John Doe", LicenseType = "Gold Mining", Status = "Pending Review", SubmittedAt = DateTime.Now.AddDays(-2) },
                    new StaffApplicationVm { Id = 2, ApplicantName = "Jane Smith", LicenseType = "Exploration License", Status = "Approved", SubmittedAt = DateTime.Now.AddDays(-7) }
                },
                Notifications = new List<string>
                {
                    "2 applications pending review.",
                    "System maintenance scheduled for tomorrow 9:00 AM."
                }
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
