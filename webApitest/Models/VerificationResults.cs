using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApitest.Models
{
    [Table("VerificationResults")]
    public class VerificationResults
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UploadId { get; set; }

        // Each column = 1 if match, 0 if mismatch
        public bool? AadhaarNameMatch { get; set; }
        public bool? AadhaarNoMatch { get; set; }
        public bool? DOBMatch { get; set; }
        public bool? PANNameMatch { get; set; }
        public bool? PANNoMatch { get; set; }
        public bool? ApplicationNumberMatch { get; set; }
        public bool? ApplicantNameMatch { get; set; }
        public bool? ApplicantAddressMatch { get; set; }
        public bool? SurveyNoMatch { get; set; }
        public bool? MeasuringAreaMatch { get; set; }
        public bool? VillageMatch { get; set; }
        public bool? HobliMatch { get; set; }
        public bool? TalukMatch { get; set; }
        public bool? DistrictMatch { get; set; }

        // Overall verification
        public bool? OverallMatch { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending / Verified / Rejected

        [Column(TypeName = "decimal(5,2)")]
        public decimal RiskScore { get; set; } = 0.00m;

        public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("UploadId")]
        public virtual UserUploadedDocuments? UserUploadedDocuments { get; set; }
    }
}
