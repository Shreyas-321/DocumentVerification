using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApitest.Models
{
    [Table("OriginalAadhaarData")]
    public class OriginalAadhaarData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string AadhaarName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "date")]
        public DateTime DOB { get; set; }

        [Required]
        [MaxLength(50)]
        public string AadhaarNo { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
