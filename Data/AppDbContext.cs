using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EMiningLicense.Models;

namespace EMiningLicense.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<LicenseApplication> LicenseApplications { get; set; }

        // You’ll add your domain entities here later (LicenseApplication, Payment, Certificate, etc.)
        // public DbSet<LicenseApplication> LicenseApplications { get; set; }
    }

}
