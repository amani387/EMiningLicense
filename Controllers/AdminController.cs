using EMiningLicense.Data;
using EMiningLicense.Models;
using EMiningLicense.Services;
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
        private readonly EmailService _emailService;

        public AdminController(AppDbContext context, UserManager<ApplicationUser> userManager, EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        private async Task SendOtpEmailAsync(string email, string otp)
        {
            var subject = "Your E-Mining License OTP Code";
            var body = $"<h2>Welcome to E-Mining License Platform</h2>" +
                       $"<p>Your verification code is: <strong>{otp}</strong></p>" +
                       $"<p>This code will expire in 10 minutes.</p>";

            await _emailService.SendEmailAsync(email, subject, body);
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
                IsApprovedByAdmin = true,   
                IsVerifiedByOtp = true     
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
        [HttpPost]
        public async Task<IActionResult> ApproveUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsApprovedByAdmin = true;
            await _userManager.UpdateAsync(user);

            // Send email notification
            await _emailService.SendEmailAsync(
                user.Email,
                "✅ Your account has been approved",
                $"Hello {user.FullName},<br/><br/>" +
                "Your account has been <b>approved by the administrator</b>. " +
                "You can now log in using your email and password.<br/><br/>" +
                "Best regards,<br/>Mining License System"
            );

            TempData["msg"] = $"✅ {user.Email} approved successfully.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> RejectUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var email = user.Email;
            var fullName = user.FullName;

            await _userManager.DeleteAsync(user);

            // Send rejection email
            await _emailService.SendEmailAsync(
                email,
                "❌ Your registration request was rejected",
                $"Hello {fullName},<br/><br/>" +
                "We’re sorry to inform you that your account registration request " +
                "has been <b>rejected</b> by the administrator.<br/><br/>" +
                "If you believe this is a mistake, please contact support.<br/><br/>" +
                "Best regards,<br/>Mining License System"
            );

            TempData["msg"] = $"❌ {email} rejected.";
            return RedirectToAction("Dashboard");
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
      .Where(u => u.IsVerifiedByOtp && !u.IsApprovedByAdmin)
      .Select(u => new PendingUserVm
      {
          Id = u.Id,
          FullName = u.FullName,
          Email = u.Email,
          OrganizationName = u.Organization ?? "",
          RegistrationDate = u.CreatedAt
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

   

        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }
    }
}
