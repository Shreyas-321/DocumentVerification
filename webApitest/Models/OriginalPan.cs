using System.ComponentModel.DataAnnotations;

namespace webApitest.Models
{
    public class OriginalPan
    {
        public int Id { get; set; }
        
        [StringLength(255)]
        public string? Name { get; set; }
        
        [StringLength(20)]
        public string? Dob { get; set; }
        
        [StringLength(20)]
        public string? PanNo { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
