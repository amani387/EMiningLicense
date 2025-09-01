namespace EMiningLicense.Models
{
    public class StaffDashboardVm
    {
        public List<string> Notifications { get; set; } = new();
        public List<AssignedApplicationVm> AssignedApplications { get; set; } = new();
        public List<LicenseApplicationVm> NewApplications { get; set; } = new();
    }
    public class LicenseApplicationVm
    {
        public int Id { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }

        // GIS + Document support
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? DocumentPath { get; set; }
        public string Status { get; set; } = "Pending";
    }
    // Existing "assigned" app type
    public class AssignedApplicationVm
    {
        public string CompanyName { get; set; }
        public int Id { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; } = "Pending";
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? DocumentPath { get; set; }
    }
    public class StaffApplicationVm
    {
        public int Id { get; set; }
        public string ApplicantName { get; set; }
        public string LicenseType { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
