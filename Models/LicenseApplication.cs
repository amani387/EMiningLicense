using System;

namespace EMiningLicense.Models
{
    public class LicenseApplication
    {
        public int Id { get; set; }
        public string ApplicationType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";  // Pending, Approved, Rejected, Under Review
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key to User
        public string ApplicantId { get; set; }
        public ApplicationUser Applicant { get; set; }
    }
}
