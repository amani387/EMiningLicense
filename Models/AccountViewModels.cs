using System.ComponentModel.DataAnnotations;

namespace EMiningLicense.Models
{
    public class RegisterVm
    {
        [Required] public string FullName { get; set; } = "";
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required] public string Phone { get; set; } = "";
        public string? Organization { get; set; }
        [Required, MinLength(6)] public string Password { get; set; } = "";
        [Compare(nameof(Password))] public string ConfirmPassword { get; set; } = "";
    }

    public class LoginVm
    {
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required] public string Password { get; set; } = "";
        public bool RememberMe { get; set; }
    }

    public class OtpVerifyVm
    {
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required] public string Code { get; set; } = "";
    }
}
