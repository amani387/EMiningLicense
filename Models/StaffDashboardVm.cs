namespace EMiningLicense.Models
{
    public class StaffDashboardVm
    {
        public List<StaffApplicationVm> AssignedApplications { get; set; } = new();
        public List<string> Notifications { get; set; } = new();
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
