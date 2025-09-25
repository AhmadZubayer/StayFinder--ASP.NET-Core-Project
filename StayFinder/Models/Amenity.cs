using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StayFinder.Models
{
    [Table("amenities")]
    public class Amenity
    {
        [Key]
        [Column("amenity_id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Column("icon")]
        [MaxLength(50)]
        public string? Icon { get; set; }

        [Column("category")]
        [MaxLength(50)]
        public string Category { get; set; } = "General"; // General, Kitchen, Bathroom, Entertainment, etc.

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
    }

    [Table("property_amenities")]
    public class PropertyAmenity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("property_id")]
        public int PropertyId { get; set; }

        [Required]
        [Column("amenity_id")]
        public int AmenityId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; } = null!;

        [ForeignKey("AmenityId")]
        public virtual Amenity Amenity { get; set; } = null!;
    }

    public class AmenityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class AmenityCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Icon { get; set; }

        [MaxLength(50)]
        public string Category { get; set; } = "General";
    }
}