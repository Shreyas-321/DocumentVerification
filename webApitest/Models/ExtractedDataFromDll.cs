using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApitest.Models
{
    public class ExtractedDataFromDll
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? ApplicationNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? SurveyNumber { get; set; }

        [MaxLength(100)]
        public string? MeasuringArea { get; set; }

        [MaxLength(100)]
        public string? Village { get; set; }

        [MaxLength(100)]
        public string? Hobli { get; set; }

        [MaxLength(100)]
        public string? Taluk { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        [MaxLength(12)]
        public string? AadhaarNumber { get; set; }

        [MaxLength(10)]
        public string? PANNumber { get; set; }

        public DateTime? DOB { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
