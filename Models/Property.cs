using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StayFinderAPI.Models
{
    public class Property
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerNight { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal Rating { get; set; }

        public int ReviewCount { get; set; }

        public int? Bedrooms { get; set; }

        public int? Bathrooms { get; set; }

        public int? MaxGuests { get; set; }

        [StringLength(50)]
        public string? PropertyType { get; set; }

        public string? Amenities { get; set; } // JSON string

        public bool IsActive { get; set; } = true;

        public bool IsSuperhost { get; set; } = false;

        public bool IsNew { get; set; } = false;

        public bool IsPopular { get; set; } = false;

        public bool IsLuxury { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    }
}