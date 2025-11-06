using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApitest.Models
{
    [Table("OriginalPANData")]
    public class OriginalPANData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string PANName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PANNo { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime? DOB { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
