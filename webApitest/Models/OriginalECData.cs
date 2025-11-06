using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApitest.Models
{
    [Table("OriginalECData")]
    public class OriginalECData
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? SurveyNo { get; set; }
        [MaxLength (50)]
        public string? ApplicationNumber {  get; set; }

        [MaxLength(50)]
        public string? MeasuringArea { get; set; }

        [MaxLength(100)]
        public string? Village { get; set; }

        [MaxLength(100)]
        public string? Hobli { get; set; }

        [MaxLength(100)]
        public string? Taluk { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? Longitude { get; set; }

        public bool Checked { get; set; } = false;

        [MaxLength(200)]
        public string? OwnerName { get; set; }

        [MaxLength(50)]
        public string? Extent { get; set; }

        public bool IsMainOwner { get; set; } = false;

        [MaxLength(50)]
        public string? OwnershipType { get; set; }

        [MaxLength(50)]
        public string? LandType { get; set; }

        public bool IsGovtRestricted { get; set; } = false;

        public bool IsCourtStay { get; set; } = false;

        public bool IsAlienated { get; set; } = false;

        public bool AnyTransaction { get; set; } = false;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}