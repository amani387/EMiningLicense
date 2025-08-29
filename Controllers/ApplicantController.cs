using EMiningLicense.Data;
using EMiningLicense.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Applicant")]
public class ApplicantController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userMgr;
    private readonly IWebHostEnvironment _env;

    public ApplicantController(AppDbContext context, UserManager<ApplicationUser> userMgr, IWebHostEnvironment env)
    {
        _context = context;
        _userMgr = userMgr;
        _env = env;
    }

    // ✅ List all applications for logged-in applicant
    public async Task<IActionResult> Index()
    {
        var user = await _userMgr.GetUserAsync(User);
        var apps = await _context.LicenseApplications
            .Where(a => a.ApplicantId == user.Id)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();

        return View(apps);
    }

    // ✅ GET: New Application Form
    [HttpGet]
    public IActionResult Apply()
    {
        return View();
    }

    // ✅ POST: Save New Application
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(LicenseApplication model, IFormFile document)
    {
        var user = await _userMgr.GetUserAsync(User);
        if (user == null) return Unauthorized();

        if (!ModelState.IsValid)
            return View(model);

        // Save document
        string? docPath = null;
        if (document != null && document.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(document.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await document.CopyToAsync(stream);
            }

            docPath = "/uploads/" + fileName; // store relative path for later retrieval
        }

        var application = new LicenseApplication
        {
            ApplicantId = user.Id,
            CompanyName = model.CompanyName,
            ContactPerson = model.ContactPerson,
            ContactEmail = model.ContactEmail,
            ContactPhone = model.ContactPhone,
            MiningType = model.MiningType,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            DocumentPath = docPath,
            AdditionalNotes = model.AdditionalNotes,
            Status = "Pending",
            SubmittedAt = DateTime.UtcNow
        };

        _context.LicenseApplications.Add(application);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Your application has been submitted successfully!";
        return RedirectToAction(nameof(Index));
    }

    // ✅ View details of a single application
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userMgr.GetUserAsync(User);
        var app = await _context.LicenseApplications
            .FirstOrDefaultAsync(a => a.Id == id && a.ApplicantId == user.Id);

        if (app == null) return NotFound();

        return View(app);
    }
}
