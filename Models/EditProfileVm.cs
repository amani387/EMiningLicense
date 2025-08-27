using System.ComponentModel.DataAnnotations;

namespace EMiningLicense.Models
{
    public class EditProfileVm
    {
        [Required, Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone, Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Display(Name = "Organization")]
        public string Organization { get; set; }
    }
}
