using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StayFinder.Models
{
    [Table("property_images")]
    public class PropertyImage
    {
        [Key]
        [Column("image_id")]
        public int Id { get; set; }

        [Required]
        [Column("property_id")]
        public int PropertyId { get; set; }

        [Required]
        [Column("image_url")]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [Column("alt_text")]
        [MaxLength(200)]
        public string? AltText { get; set; }

        [Column("is_primary")]
        public bool IsPrimary { get; set; } = false;

        [Column("display_order")]
        public int DisplayOrder { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; } = null!;
    }

    public class PropertyImageDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PropertyImageCreateDto
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? AltText { get; set; }

        public bool IsPrimary { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;
    }
}