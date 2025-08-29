using EMiningLicense.Models;
using EMiningLicense.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EMiningLicense.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly SignInManager<ApplicationUser> _signInMgr;
        private readonly EmailService _emailService;

        public AccountController(
            UserManager<ApplicationUser> userMgr,
            SignInManager<ApplicationUser> signInMgr,
            EmailService emailService)
        {
            _emailService = emailService;
            _userMgr = userMgr;
            _signInMgr = signInMgr;
        }

        // -------------------- REGISTER --------------------
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Generate OTP first
            var otp = new Random().Next(100000, 999999).ToString();

            try
            {
                // Try sending OTP BEFORE creating the user
                await _emailService.SendEmailAsync(
                    vm.Email,
                    "Your OTP Code",
                    $"Your verification code is <b>{otp}</b>. It expires in 10 minutes."
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Email sending failed: " + ex.Message);
                ModelState.AddModelError("", "Could not send OTP email. Please try again later.");
                return View(vm); // stop here, don’t create the user
            }

            // If email sent successfully → create the user
            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                FullName = vm.FirstName,
                Organization = vm.OrganizationName,
                EmailConfirmed = false,
                IsApprovedByAdmin = false,
                IsVerifiedByOtp = false,
                OtpCode = otp,
                OtpExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            var result = await _userMgr.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            // Default role = Applicant
            await _userMgr.AddToRoleAsync(user, "Applicant");

            // ✅ Now navigate user to OTP verification page
            return RedirectToAction(nameof(VerifyOtp), new { email = vm.Email });
        }

        // -------------------- VERIFY OTP --------------------
        [HttpGet]
        public IActionResult VerifyOtp(string email)
        {
            var vm = new OtpVerifyVm { Email = email };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(OtpVerifyVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userMgr.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid user.");
                return View(vm);
            }

            if (string.IsNullOrEmpty(user.OtpCode) || user.OtpExpiresAt == null)
            {
                ModelState.AddModelError("", "OTP not generated. Please register again.");
                return View(vm);
            }

            if (user.OtpCode == vm.Code && user.OtpExpiresAt > DateTimeOffset.UtcNow)
            {
                user.IsVerifiedByOtp = true;
                user.EmailConfirmed = true;
                user.OtpCode = null;
                user.OtpExpiresAt = null;
                await _userMgr.UpdateAsync(user);

                // Show pending approval screen instead of login
                return RedirectToAction(nameof(PendingApproval));
            }

            ModelState.AddModelError("", "Invalid or expired OTP code.");
            return View(vm);
        }

        // -------------------- LOGIN --------------------
        [HttpGet]
        public IActionResult PendingApproval() => View();

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVm vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userMgr.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(vm);
            }

            var result = await _signInMgr.PasswordSignInAsync(user, vm.Password, vm.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (!user.IsVerifiedByOtp)
                    return RedirectToAction(nameof(VerifyOtp), new { email = user.Email });

                if (!user.IsApprovedByAdmin)
                    return RedirectToAction(nameof(PendingApproval));

                // Redirect based on role
                if (await _userMgr.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Dashboard", "Admin");

                if (await _userMgr.IsInRoleAsync(user, "Staff"))
                    return RedirectToAction("Index", "Staff");

                if (await _userMgr.IsInRoleAsync(user, "Applicant"))
                    return RedirectToAction("Index", "Applicant");

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
                ModelState.AddModelError("", "Your account is locked out due to too many failed attempts.");
            else if (result.IsNotAllowed)
                ModelState.AddModelError("", "You must verify your email/OTP or wait for admin approval before logging in.");
            else if (result.RequiresTwoFactor)
                ModelState.AddModelError("", "Two-factor authentication is required.");
            else
                ModelState.AddModelError("", "Invalid login attempt.");

            return View(vm);
        }

        // -------------------- LOGOUT --------------------
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInMgr.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Denied() => View();
    }
}
