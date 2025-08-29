using System;

namespace EMiningLicense.Models
{
    public class LicenseApplication
    {
        public int Id { get; set; }
        public string ApplicationType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Applicant linkage
        public string ApplicantId { get; set; }
        public ApplicationUser Applicant { get; set; }

        // Company / Personal Data
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }

        // Mining Activity
        public string MiningType { get; set; }   // e.g. Gold, Coal, Quarry
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // GIS Location (lat/lon for map pin)
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Uploaded Docs
        public string DocumentPath { get; set; } // store filename or path
        public string AdditionalNotes { get; set; }

        // Status & Timestamps
        public string Status { get; set; } = "Pending"; // Pending/Approved/Rejected
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public bool IsRenewal { get; set; } = false;
    }

}
