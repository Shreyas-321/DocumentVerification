using System.ComponentModel.DataAnnotations;

namespace webApitest.Models
{
    public class OriginalDocuments
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        // Original document file paths
        public string? OriginalECPath { get; set; }
        public string? OriginalAadhaarPath { get; set; }
        public string? OriginalPANPath { get; set; }
        
        // Original extracted data for comparison
        public string? OriginalExtractedData { get; set; } // JSON string of original extracted fields
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
