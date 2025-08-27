using System.ComponentModel.DataAnnotations;
namespace EMiningLicense.Models
{
    public class CreateStaffVm
    {
        [Required]
        public string FullName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}