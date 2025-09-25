using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StayFinder.Models
{
    [Table("properties")]
    public class Property
    {
        [Key]
        [Column("property_id")]
        public int Id { get; set; }

        [Required]
        [Column("host_id")]
        public int HostId { get; set; }

        [Required]
        [Column("title")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("location")]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Column("city")]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Column("state")]
        [MaxLength(100)]
        public string? State { get; set; }

        [Column("country")]
        [MaxLength(100)]
        public string Country { get; set; } = "USA";

        [Column("zip_code")]
        [MaxLength(20)]
        public string? ZipCode { get; set; }

        [Column("latitude")]
        [Precision(10, 8)]
        public decimal? Latitude { get; set; }

        [Column("longitude")]
        [Precision(11, 8)]
        public decimal? Longitude { get; set; }

        [Column("description")]
        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        [Column("type")]
        [MaxLength(50)]
        public string PropertyType { get; set; } = string.Empty;

        [Column("bedrooms")]
        public int Bedrooms { get; set; } = 1;

        [Column("bathrooms")]
        public int Bathrooms { get; set; } = 1;

        [Column("max_guests")]
        public int MaxGuests { get; set; } = 1;

        [Column("size_sqft")]
        public int? SizeSquareFeet { get; set; }

        [Column("rating")]
        [Precision(3, 2)]
        public decimal? Rating { get; set; }

        [Column("review_count")]
        public int ReviewCount { get; set; } = 0;

        [Required]
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Column("cleaning_fee", TypeName = "decimal(10,2)")]
        public decimal? CleaningFee { get; set; }

        [Column("security_deposit", TypeName = "decimal(10,2)")]
        public decimal? SecurityDeposit { get; set; }

        [Required]
        [Column("available_from")]
        public DateTime AvailableFrom { get; set; }

        [Required]
        [Column("available_to")]
        public DateTime AvailableTo { get; set; }

        [Column("check_in_time")]
        public TimeOnly? CheckInTime { get; set; }

        [Column("check_out_time")]
        public TimeOnly? CheckOutTime { get; set; }

        [Column("minimum_stay")]
        public int MinimumStay { get; set; } = 1;

        [Column("maximum_stay")]
        public int? MaximumStay { get; set; }

        [Column("is_instant_book")]
        public bool IsInstantBook { get; set; } = false;

        [Column("is_approved")]
        public bool IsApproved { get; set; } = false;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("house_rules")]
        [MaxLength(1000)]
        public string? HouseRules { get; set; }

        [Column("cancellation_policy")]
        [MaxLength(50)]
        public string CancellationPolicy { get; set; } = "Moderate";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Legacy field for backward compatibility
        [Column("image_path")]
        [MaxLength(500)]
        public string? ImagePath { get; set; }

        // Navigation properties
        [ForeignKey("HostId")]
        public virtual User Host { get; set; } = null!;

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
        public virtual ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();
        public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
    }

    public class PropertyDto
    {
        public int Id { get; set; }
        public int HostId { get; set; }
        public string HostName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string Country { get; set; } = string.Empty;
        public string? ZipCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Description { get; set; }
        public string PropertyType { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int MaxGuests { get; set; }
        public int? SizeSquareFeet { get; set; }
        public decimal? Rating { get; set; }
        public int ReviewCount { get; set; }
        public decimal Price { get; set; }
        public decimal? CleaningFee { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public int MinimumStay { get; set; }
        public int? MaximumStay { get; set; }
        public bool IsInstantBook { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public string? HouseRules { get; set; }
        public string CancellationPolicy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<string> AmenityNames { get; set; } = new List<string>();
        public List<PropertyImageDto> Images { get; set; } = new List<PropertyImageDto>();
        public string? ImagePath { get; set; } // For backward compatibility
    }

    public class PropertyCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(100)]
        public string Country { get; set; } = "USA";

        [MaxLength(20)]
        public string? ZipCode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string PropertyType { get; set; } = string.Empty;

        [Range(1, 20, ErrorMessage = "Bedrooms must be between 1 and 20")]
        public int Bedrooms { get; set; } = 1;

        [Range(1, 20, ErrorMessage = "Bathrooms must be between 1 and 20")]
        public int Bathrooms { get; set; } = 1;

        [Range(1, 50, ErrorMessage = "Max guests must be between 1 and 50")]
        public int MaxGuests { get; set; } = 1;

        public int? SizeSquareFeet { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public decimal? CleaningFee { get; set; }

        public decimal? SecurityDeposit { get; set; }

        [Required]
        public DateTime AvailableFrom { get; set; }

        [Required]
        public DateTime AvailableTo { get; set; }

        public TimeOnly? CheckInTime { get; set; }

        public TimeOnly? CheckOutTime { get; set; }

        [Range(1, 365, ErrorMessage = "Minimum stay must be between 1 and 365 days")]
        public int MinimumStay { get; set; } = 1;

        public int? MaximumStay { get; set; }

        public bool IsInstantBook { get; set; } = false;

        [MaxLength(1000)]
        public string? HouseRules { get; set; }

        [MaxLength(50)]
        public string CancellationPolicy { get; set; } = "Moderate";

        public List<int> AmenityIds { get; set; } = new List<int>();

        public List<string> ImageUrls { get; set; } = new List<string>();
    }

    public class PropertyUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(100)]
        public string Country { get; set; } = "USA";

        [MaxLength(20)]
        public string? ZipCode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string PropertyType { get; set; } = string.Empty;

        [Range(1, 20, ErrorMessage = "Bedrooms must be between 1 and 20")]
        public int Bedrooms { get; set; } = 1;

        [Range(1, 20, ErrorMessage = "Bathrooms must be between 1 and 20")]
        public int Bathrooms { get; set; } = 1;

        [Range(1, 50, ErrorMessage = "Max guests must be between 1 and 50")]
        public int MaxGuests { get; set; } = 1;

        public int? SizeSquareFeet { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public decimal? CleaningFee { get; set; }

        public decimal? SecurityDeposit { get; set; }

        [Required]
        public DateTime AvailableFrom { get; set; }

        [Required]
        public DateTime AvailableTo { get; set; }

        public TimeOnly? CheckInTime { get; set; }

        public TimeOnly? CheckOutTime { get; set; }

        [Range(1, 365, ErrorMessage = "Minimum stay must be between 1 and 365 days")]
        public int MinimumStay { get; set; } = 1;

        public int? MaximumStay { get; set; }

        public bool IsInstantBook { get; set; } = false;

        public bool IsActive { get; set; } = true;

        [MaxLength(1000)]
        public string? HouseRules { get; set; }

        [MaxLength(50)]
        public string CancellationPolicy { get; set; } = "Moderate";

        public List<int> AmenityIds { get; set; } = new List<int>();

        public List<string> ImageUrls { get; set; } = new List<string>();
    }

    public class PropertySearchDto
    {
        public string? Location { get; set; }
        public string? City { get; set; }
        public string? PropertyType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? Guests { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public List<int> AmenityIds { get; set; } = new List<int>();
        public decimal? MinRating { get; set; }
        public bool? IsInstantBook { get; set; }
        public string SortBy { get; set; } = "price"; // price, rating, created
        public string SortOrder { get; set; } = "asc"; // asc, desc
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}