using System.ComponentModel.DataAnnotations;

namespace VerificationDLL.Models
{
    public class ExtractedData
    {
        public int Id { get; set; }
        public int UploadId { get; set; }

        // EC Data
        public string? ApplicationNumber { get; set; }
        public string? ApplicantName { get; set; }
        public string? ApplicantAddress { get; set; }
        public string? SurveyNo { get; set; }
        public string? MeasuringArea { get; set; }
        public string? Village { get; set; }
        public string? Hobli { get; set; }
        public string? Taluk { get; set; }
        public string? District { get; set; }

        // Aadhaar Data
        public string? AadhaarName { get; set; }
        public string? AadhaarNo { get; set; }
        public string? DOB { get; set; }

        // PAN Data
        public string? PANName { get; set; }
        public string? PANNo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}