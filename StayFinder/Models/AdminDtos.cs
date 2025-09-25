namespace StayFinder.Models
{
    // Analytics DTOs
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalHosts { get; set; }
        public int PendingHostApprovals { get; set; }
        public int TotalProperties { get; set; }
        public int PendingPropertyApprovals { get; set; }
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal TotalCommission { get; set; }
    }

    public class UserAnalyticsDto
    {
        public int TotalUsers { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; } = new();
        public int NewUsersThisMonth { get; set; }
        public int ActiveUsersThisMonth { get; set; }
        public Dictionary<string, int> UserRegistrationsByMonth { get; set; } = new();
    }

    public class PropertyAnalyticsDto
    {
        public int TotalProperties { get; set; }
        public Dictionary<string, int> PropertiesByType { get; set; } = new();
        public Dictionary<string, int> PropertiesByStatus { get; set; } = new();
        public int NewPropertiesThisMonth { get; set; }
        public Dictionary<string, int> PropertyListingsByMonth { get; set; } = new();
        public List<PropertyDto> TopRatedProperties { get; set; } = new();
        public List<PropertyDto> MostBookedProperties { get; set; } = new();
    }

    public class BookingAnalyticsDto
    {
        public int TotalBookings { get; set; }
        public Dictionary<string, int> BookingsByStatus { get; set; } = new();
        public int NewBookingsThisMonth { get; set; }
        public Dictionary<string, int> BookingsByMonth { get; set; } = new();
        public decimal AverageBookingValue { get; set; }
        public List<object> TopBookingDestinations { get; set; } = new();
        public object BookingTrends { get; set; } = new();
    }

    public class RevenueAnalyticsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal MonthlyCommission { get; set; }
        public Dictionary<string, decimal> RevenueByMonth { get; set; } = new();
        public Dictionary<string, decimal> RevenueByPropertyType { get; set; } = new();
        public List<object> TopRevenueGeneratingProperties { get; set; } = new();
        public List<object> TopRevenueGeneratingHosts { get; set; } = new();
    }

    // Bulk Operation DTOs
    public class BulkUserOperationDto
    {
        public List<int> UserIds { get; set; } = new();
        public string Operation { get; set; } = string.Empty; // "approve", "reject", "activate", "deactivate"
        public string? Reason { get; set; }
    }

    public class BulkPropertyOperationDto
    {
        public List<int> PropertyIds { get; set; } = new();
        public string Operation { get; set; } = string.Empty; // "approve", "reject", "activate", "deactivate"
        public string? Reason { get; set; }
    }

    // System Settings DTO
    public class SystemSettingsDto
    {
        public decimal CommissionRate { get; set; }
        public int MaxBookingDays { get; set; }
        public int MinBookingDays { get; set; }
        public string CancellationPolicy { get; set; } = string.Empty;
        public string[] PaymentMethods { get; set; } = Array.Empty<string>();
        public string[] SupportedCurrencies { get; set; } = Array.Empty<string>();
        public long MaxImageUploadSize { get; set; }
        public string[] AllowedImageTypes { get; set; } = Array.Empty<string>();
    }

    // Activity Log DTO
    public class ActivityLogDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Details { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    // User Update DTO
    public class UserUpdateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
    }

    // Offer Application DTO
    public class OfferApplicationDto
    {
        public int BookingId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Guests { get; set; }
    }

    // Property Search Request
    public class PropertySearchRequest
    {
        public string? Destination { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? Guests { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? PropertyType { get; set; }
        public List<string>? Amenities { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}