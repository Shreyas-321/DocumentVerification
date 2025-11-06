using System.ComponentModel.DataAnnotations;

namespace webApitest.Models
{
    public class ExtractedData
    {
        public int Id { get; set; }
        
        [Required]
        public int UploadId { get; set; }
        public UserUploadedDocuments UserUploadedDocuments { get; set; } = null!;
        
        // Aadhaar extracted data
        public string? AadhaarName { get; set; }
        public string? AadhaarNo { get; set; }
        public string? DOB { get; set; }
        
        // PAN extracted data
        public string? PANName { get; set; }
        public string? PANNo { get; set; }
        public string? PANDOB { get; set; }
        
        // Application data
        public string? ApplicationNumber { get; set; }
        public string? ApplicantName { get; set; }
        public string? ApplicantAddress { get; set; }
        
        // Survey data
        public string SurveyNo { get; set; }
        public string? MeasuringArea { get; set; }
        public string? Village { get; set; }
        public string? Hobli { get; set; }
        public string? Taluk { get; set; }
        public string? District { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
