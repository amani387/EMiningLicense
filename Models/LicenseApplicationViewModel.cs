using System.ComponentModel.DataAnnotations;

namespace EMiningLicense.Models
{
    public class LicenseApplicationViewModel
    {
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string ContactPerson { get; set; }

        [Required, EmailAddress]
        public string ContactEmail { get; set; }

        [Required]
        public string ContactPhone { get; set; }

        [Required]
        public string MiningType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string? AdditionalNotes { get; set; }
    }
}
