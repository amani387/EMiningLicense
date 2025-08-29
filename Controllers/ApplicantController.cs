using EMiningLicense.Data;
using EMiningLicense.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Applicant")]
public class ApplicantController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userMgr;

    public ApplicantController(AppDbContext db, UserManager<ApplicationUser> userMgr)
    {
        _db = db;
        _userMgr = userMgr;
    }

    // GET: My Applications
    public async Task<IActionResult> Index()
    {
        var user = await _userMgr.GetUserAsync(User);
        var apps = await _db.LicenseApplications
            .Where(a => a.ApplicantId == user.Id)
            .ToListAsync();

        return View(apps);
    }

    // GET: Apply form
    [HttpGet]
    public IActionResult Apply()
    {
        return View(new LicenseApplication());
    }

    // POST: Submit new application
    [HttpPost]
    public async Task<IActionResult> Apply(LicenseApplication model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userMgr.GetUserAsync(User);
        model.ApplicantId = user.Id;
        model.SubmittedAt = DateTime.UtcNow;
        model.Status = "Pending";

        _db.LicenseApplications.Add(model);
        await _db.SaveChangesAsync();

        TempData["msg"] = "✅ Application submitted successfully!";
        return RedirectToAction("Index");
    }

    // GET: View application details
    public async Task<IActionResult> Details(int id)
    {
        var app = await _db.LicenseApplications
            .Include(a => a.Applicant)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null) return NotFound();
        return View(app);
    }
}
