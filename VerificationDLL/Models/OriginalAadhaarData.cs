using System.ComponentModel.DataAnnotations;

namespace VerificationDLL.Models
{
    public class OriginalAadhaarData
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string AadhaarName { get; set; } = null!;

        [Required]
        public string DOB { get; set; }

        [Required]
        [MaxLength(50)]
        public string AadhaarNo { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
