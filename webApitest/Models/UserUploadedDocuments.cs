using System.ComponentModel.DataAnnotations;

namespace webApitest.Models
{
    public class UserUploadedDocuments
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        // Document file paths
        public string? ECPath { get; set; }
        public string? AadhaarPath { get; set; }
        public string? PANPath { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}