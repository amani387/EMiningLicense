using Microsoft.AspNetCore.Identity;
using System;

namespace EMiningLicense.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string FullName { get; set; } = "";
        public string? Organization { get; set; }

        // Business rules
        public bool IsApprovedByAdmin { get; set; } = false; // Gate login until approved
        public bool IsVerifiedByOtp { get; set; } = false;

        // Simple OTP fields (you’ll replace with a proper provider later)
        public string? OtpCode { get; set; }
        public DateTimeOffset? OtpExpiresAt { get; set; }
    }
}
