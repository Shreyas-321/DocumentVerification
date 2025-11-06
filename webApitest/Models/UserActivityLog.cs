using System.ComponentModel.DataAnnotations;

namespace webApitest.Models
{
    public class UserActivityLog
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        [StringLength(100)]
        public string Activity { get; set; } = string.Empty; // "Login", "Document Upload", "Verification", etc.
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(50)]
        public string? IPAddress { get; set; }
        
        [StringLength(200)]
        public string? UserAgent { get; set; }
        
        // New fields for better audit tracking
        [StringLength(50)]
        public string? RelatedEntityType { get; set; } // "Upload", "Extraction", "Verification", etc.
        
        public int? RelatedEntityId { get; set; } // ID of the related entity (UploadId, ExtractedDataId, etc.)
        
        [StringLength(100)]
        public string? ActionResult { get; set; } // "Success", "Failed", "Pending", etc.
        
        [StringLength(1000)]
        public string? AdditionalData { get; set; } // JSON string for additional context
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
