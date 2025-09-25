using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StayFinder.Models
{
    [Table("bookings")]
    public class Booking
    {
        [Key]
        [Column("booking_id")]
        public int Id { get; set; }

        [Required]
        [Column("booking_reference")]
        [MaxLength(50)]
        public string BookingReference { get; set; } = string.Empty;

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
        public int Guests { get; set; }

        [Required]
        [Column("total_price", TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Column("status")]
        [MaxLength(50)]
        public string Status { get; set; } = "Confirmed";

        [Column("booking_notes")]
        public string? BookingNotes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("offer_id")]
        public int? OfferId { get; set; }

        [Column("discount_amount", TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column("cleaning_fee", TypeName = "decimal(10,2)")]
        public decimal CleaningFee { get; set; } = 0;

        [Column("security_deposit", TypeName = "decimal(10,2)")]
        public decimal SecurityDeposit { get; set; } = 0;

        [Column("host_confirmed")]
        public bool HostConfirmed { get; set; } = true;

        [Column("payment_status")]
        [MaxLength(50)]
        public string PaymentStatus { get; set; } = "Pending";

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; } = null!;

        [ForeignKey("OfferId")]
        public virtual Offer? Offer { get; set; }

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        // Calculated property
        public int TotalDays => (CheckOut - CheckIn).Days;
    }

    public class BookingDto
    {
        public int Id { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int PropertyId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Guests { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? BookingNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalDays { get; set; }

        // Related data
        public PropertyDto? Property { get; set; }
        public UserDto? User { get; set; }

        public static BookingDto FromEntity(Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                BookingReference = booking.BookingReference,
                UserId = booking.UserId,
                PropertyId = booking.PropertyId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                Guests = booking.Guests,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                BookingNotes = booking.BookingNotes,
                CreatedAt = booking.CreatedAt,
                TotalDays = booking.TotalDays,
                Property = booking.Property != null ? new PropertyDto 
                { 
                    Id = booking.Property.Id, 
                    Title = booking.Property.Title,
                    Location = booking.Property.Location,
                    City = booking.Property.City,
                    Price = booking.Property.Price,
                    PropertyType = booking.Property.PropertyType
                } : null,
                User = booking.User != null ? UserDto.FromEntity(booking.User) : null
            };
        }
    }

    public class BookingCreateDto
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        [Required]
        [Range(1, 20)]
        public int Guests { get; set; }

        public string? BookingNotes { get; set; }
    }

    public class BookingUpdateDto
    {
        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        [Required]
        [Range(1, 20)]
        public int Guests { get; set; }

        public string? BookingNotes { get; set; }

        public string Status { get; set; } = "Confirmed";
    }
}