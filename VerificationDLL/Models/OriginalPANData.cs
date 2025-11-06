using System.ComponentModel.DataAnnotations;

namespace VerificationDLL.Models
{
    public class OriginalPANData
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string PANName { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string PANNo { get; set; } = null!;

        public string DOB { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}