using EMiningLicense.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EMiningLicense.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly SignInManager<ApplicationUser> _signInMgr;

        public AccountController(UserManager<ApplicationUser> userMgr, SignInManager<ApplicationUser> signInMgr)
        {
            _userMgr = userMgr;
            _signInMgr = signInMgr;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                PhoneNumber = vm.Phone,
                FullName = vm.FullName,
                Organization = vm.Organization,
                EmailConfirmed = false,
                IsApprovedByAdmin = false,
                IsVerifiedByOtp = false
            };

            var result = await _userMgr.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            // Generate a simple OTP (replace with SMS/Email provider)
            var otp = new Random().Next(100000, 999999).ToString();
            user.OtpCode = otp;
            user.OtpExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10);
            await _userMgr.UpdateAsync(user);

            // TODO: send OTP via Email/SMS provider
            // For dev, show a page telling user to check email/SMS — or display it (DEV ONLY)
            TempData["DevOtp"] = otp;

            // Put all new users in Applicant role by default
            await _userMgr.AddToRoleAsync(user, "Applicant");

            return RedirectToAction(nameof(VerifyOtp), new { email = vm.Email });
        }

        [HttpGet]
        public async Task<IActionResult> VerifyOtp(string email)
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

            if (user.OtpCode == vm.Code && user.OtpExpiresAt > DateTimeOffset.UtcNow)
            {
                user.IsVerifiedByOtp = true;
                user.EmailConfirmed = true; // if using email as identity
                user.OtpCode = null;
                user.OtpExpiresAt = null;
                await _userMgr.UpdateAsync(user);

                // Now wait for admin approval
                
                     return RedirectToAction(nameof(Login));
            }

            ModelState.AddModelError("", "Invalid or expired OTP code.");
            return View(vm);
        }

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

            // ⚡ Debug-friendly login attempt
            var result = await _signInMgr.PasswordSignInAsync(user, vm.Password, vm.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Redirect based on role
                if (await _userMgr.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Dashboard", "Admin");

                if (await _userMgr.IsInRoleAsync(user, "Staff"))
                    return RedirectToAction("Index", "Staff");

                if (await _userMgr.IsInRoleAsync(user, "Applicant"))
                    return RedirectToAction("Index", "Applicant");

                return RedirectToAction("Index", "Home");
            }

            // ⚡ Add proper error messages for clarity
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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInMgr.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Denied() => View();
    }
}
