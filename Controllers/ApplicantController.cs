using EMiningLicense.Data;
using EMiningLicense.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMiningLicense.Controllers
{
    [Authorize(Roles = "Applicant")]
    public class ApplicantController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userMgr;

        public ApplicantController(AppDbContext context, UserManager<ApplicationUser> userMgr)
        {
            _context = context;
            _userMgr = userMgr;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userMgr.GetUserAsync(User);

            var apps = await _context.LicenseApplications
                .Where(a => a.ApplicantId == user.Id)
                .ToListAsync();

            return View(apps);
        }

        [HttpGet]
        public IActionResult Apply()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Apply(LicenseApplication app)
        {
            var user = await _userMgr.GetUserAsync(User);
            app.ApplicantId = user.Id;
            app.Status = "Pending";
            app.SubmittedAt = DateTime.Now;

            _context.LicenseApplications.Add(app);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
