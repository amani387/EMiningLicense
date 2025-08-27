using System;
using System.Collections.Generic;

namespace EMiningLicense.Models
{
    public class AdminDashboardViewModel
    {
        // Stats
        public int TotalUsers { get; set; }
        public int PendingUserApprovals { get; set; }
        public int ActiveLicenses { get; set; }
        public int SystemAlerts { get; set; }

        // Tables
        public List<PendingUserVm> PendingUsers { get; set; } = new();
        public List<SystemActivityVm> RecentActivities { get; set; } = new();
    }

    // Small helper viewmodels
    public class PendingUserVm
    {
        public string Id { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string OrganizationName { get; set; } = "";
        public DateTime RegistrationDate { get; set; }
    }

    public class SystemActivityVm
    {
        public string Action { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}
