using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VerificationDLL.Models
{
    public class OriginalECData
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string? SurveyNo { get; set; }
        [MaxLength(50)]
        public string? ApplicationNumber { get; set; }

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

        //[Column(TypeName = "decimal(9,6)")]
        //public decimal? Latitude { get; set; }

        //[Column(TypeName = "decimal(9,6)")]
        //public decimal? Longitude { get; set; }

        public bool Checked { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
