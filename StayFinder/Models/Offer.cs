using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StayFinder.Models
{
    [Table("offers")]
    public class Offer
    {
        [Key]
        [Column("offer_id")]
        public int Id { get; set; }

        [Required]
        [Column("title")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Column("offer_type")]
        [MaxLength(50)]
        public string OfferType { get; set; } = string.Empty; // Percentage, Fixed, Package

        [Required]
        [Column("discount_value", TypeName = "decimal(10,2)")]
        public decimal DiscountValue { get; set; }

        [Column("minimum_amount", TypeName = "decimal(10,2)")]
        public decimal? MinimumAmount { get; set; }

        [Column("maximum_discount", TypeName = "decimal(10,2)")]
        public decimal? MaximumDiscount { get; set; }

        [Required]
        [Column("valid_from")]
        public DateTime ValidFrom { get; set; }

        [Required]
        [Column("valid_to")]
        public DateTime ValidTo { get; set; }

        [Column("usage_limit")]
        public int? UsageLimit { get; set; }

        [Column("used_count")]
        public int UsedCount { get; set; } = 0;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("applicable_to")]
        [MaxLength(50)]
        public string ApplicableTo { get; set; } = "All"; // All, Property, User, FirstTime

        [Column("property_id")]
        public int? PropertyId { get; set; }

        [Column("offer_code")]
        [MaxLength(20)]
        public string? OfferCode { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; } // Admin or Host ID

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("PropertyId")]
        public virtual Property? Property { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

    public class OfferDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string OfferType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal? MinimumAmount { get; set; }
        public decimal? MaximumDiscount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public bool IsActive { get; set; }
        public string ApplicableTo { get; set; } = string.Empty;
        public int? PropertyId { get; set; }
        public string? PropertyTitle { get; set; }
        public string? OfferCode { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class OfferCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string OfferType { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount value must be greater than 0")]
        public decimal DiscountValue { get; set; }

        public decimal? MinimumAmount { get; set; }

        public decimal? MaximumDiscount { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        public int? UsageLimit { get; set; }

        [MaxLength(50)]
        public string ApplicableTo { get; set; } = "All";

        public int? PropertyId { get; set; }

        [MaxLength(20)]
        public string? OfferCode { get; set; }
    }

    public class OfferUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount value must be greater than 0")]
        public decimal DiscountValue { get; set; }

        public decimal? MinimumAmount { get; set; }

        public decimal? MaximumDiscount { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        public int? UsageLimit { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(50)]
        public string ApplicableTo { get; set; } = "All";

        public int? PropertyId { get; set; }
    }
}