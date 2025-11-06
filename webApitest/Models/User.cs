using System.ComponentModel.DataAnnotations;

namespace webApitest.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string State { get; set; } = string.Empty;
        
        [Required]
        [StringLength(10)]
        public string Pincode { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "User";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}
