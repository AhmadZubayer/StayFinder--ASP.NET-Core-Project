using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StayFinder.Models
{
    [Table("cart_items")]
    public class CartItem
    {
        [Key]
        [Column("cart_item_id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("property_id")]
        public int PropertyId { get; set; }

        [Required]
        [Column("check_in")]
        public DateTime CheckIn { get; set; }

        [Required]
        [Column("check_out")]
        public DateTime CheckOut { get; set; }

        [Required]
        [Column("guests")]
        [Range(1, 50)]
        public int Guests { get; set; }

        [Column("special_requests")]
        [MaxLength(500)]
        public string? SpecialRequests { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; } = null!;

        // Calculated properties
        public int TotalDays => (CheckOut - CheckIn).Days;
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string PropertyLocation { get; set; } = string.Empty;
        public decimal PropertyPrice { get; set; }
        public string? PropertyImage { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Guests { get; set; }
        public string? SpecialRequests { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalDays { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CartItemCreateDto
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "Guests must be between 1 and 50")]
        public int Guests { get; set; }

        [MaxLength(500)]
        public string? SpecialRequests { get; set; }
    }

    public class CartItemUpdateDto
    {
        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "Guests must be between 1 and 50")]
        public int Guests { get; set; }

        [MaxLength(500)]
        public string? SpecialRequests { get; set; }
    }
}