using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApitest.Models
{
    public class OriginalSurveyNo
    {
        public int Id { get; set; }
        
        [StringLength(50)]
        public string? SurveyNo { get; set; }
        
        [StringLength(50)]
        public string? MeasuringArea { get; set; }
        
        [StringLength(100)]
        public string? Village { get; set; }
        
        [StringLength(100)]
        public string? Hobli { get; set; }
        
        [StringLength(100)]
        public string? Taluk { get; set; }
        
        [StringLength(100)]
        public string? District { get; set; }
        
        [Column(TypeName = "decimal(9,6)")]
        public decimal? Latitude { get; set; }
        
        [Column(TypeName = "decimal(9,6)")]
        public decimal? Longitude { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
