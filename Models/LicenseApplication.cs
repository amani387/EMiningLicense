using System;

namespace EMiningLicense.Models
{
    public class LicenseApplication
    {
        public int Id { get; set; }
        public string ApplicantId { get; set; } = "";
        public string LicenseType { get; set; } = "";
        public string ApplicationType { get; set; } = "";
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
