namespace StayFinderAPI.Models.DTOs
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int? MaxGuests { get; set; }
        public string PropertyType { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new List<string>();
        public bool IsSuperhost { get; set; }
        public bool IsNew { get; set; }
        public bool IsPopular { get; set; }
        public bool IsLuxury { get; set; }
        public List<PropertyImageDto> Images { get; set; } = new List<PropertyImageDto>();
        public string PrimaryImageUrl { get; set; } = string.Empty;
        public string BadgeText { get; set; } = string.Empty;
        public string BadgeClass { get; set; } = string.Empty;
    }

    public class PropertyImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }
}