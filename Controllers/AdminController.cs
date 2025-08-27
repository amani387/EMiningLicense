using EMiningLicense.Data;
using EMiningLicense.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMiningLicense.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        [HttpGet]
        public IActionResult CreateStaff() => View();

        [HttpPost]
        public async Task<IActionResult> CreateStaff(CreateStaffVm vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "⚠️ Please fill all required fields correctly.";
                return View(vm);
            }

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(vm.Email);
            if (existingUser != null)
            {
                ViewBag.Error = "❌ A user with this email already exists.";
                return View(vm);
            }

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                FullName = vm.FullName,
                EmailConfirmed = true,
                IsApprovedByAdmin = true,   // since admin is creating
                IsVerifiedByOtp = true      // staff doesn’t need OTP
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                // Pass Identity errors to UI
                ViewBag.Error = string.Join("; ", result.Errors.Select(e => e.Description));
                return View(vm);
            }

            // Add Staff role
            await _userManager.AddToRoleAsync(user, "Staff");

            TempData["msg"] = "✅ Staff account created successfully!";
            return RedirectToAction("CreateStaff"); // back to form so admin can add more
        }


        public async Task<IActionResult> Dashboard()
        {
            // Collect data
            var totalUsers = await _context.Users.CountAsync();
            var pendingApprovals = await _context.Users.CountAsync(u => !u.IsApprovedByAdmin && u.IsVerifiedByOtp);
            var activeLicenses = await _context.Set<LicenseApplication>().CountAsync(l => l.Status == "Approved"); // placeholder
            var systemAlerts = 2; // could be pulled from an Alerts table

            // Pending Users
            var pendingUsers = await _context.Users
                .Where(u => !u.IsApprovedByAdmin && u.IsVerifiedByOtp)
                .Select(u => new PendingUserVm
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    OrganizationName = u.Organization ?? "",
                    RegistrationDate = u.LockoutEnd != null ? u.LockoutEnd.Value.UtcDateTime : DateTime.UtcNow
                })
                .ToListAsync();

            // Dummy system activities for now
            var activities = new List<SystemActivityVm>
            {
                new SystemActivityVm { Action = "Login", Description = "Admin logged in", Timestamp = DateTime.Now },
                new SystemActivityVm { Action = "User Approval", Description = "Approved new applicant", Timestamp = DateTime.Now.AddMinutes(-30) },
                new SystemActivityVm { Action = "System Check", Description = "Database backup completed", Timestamp = DateTime.Now.AddHours(-1) }
            };

            var vm = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                PendingUserApprovals = pendingApprovals,
                ActiveLicenses = activeLicenses,
                SystemAlerts = systemAlerts,
                PendingUsers = pendingUsers,
                RecentActivities = activities
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsApprovedByAdmin = true;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> RejectUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }
    }
}
