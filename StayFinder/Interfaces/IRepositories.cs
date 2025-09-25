using StayFinder.Models;

namespace StayFinder.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<User>> GetHostsAsync();
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<User?> GetUserWithBookingsAsync(int userId);
        Task<User?> GetUserWithPropertiesAsync(int userId);
    }

    public interface IPropertyRepository : IRepository<Property>
    {
        Task<IEnumerable<Property>> GetByHostIdAsync(int hostId);
        Task<IEnumerable<Property>> SearchPropertiesAsync(PropertySearchDto searchDto);
        Task<int> GetSearchResultsCountAsync(PropertySearchDto searchDto);
        Task<IEnumerable<Property>> GetAvailablePropertiesAsync(DateTime checkIn, DateTime checkOut);
        Task<IEnumerable<Property>> GetPropertiesByCityAsync(string city);
        Task<IEnumerable<Property>> GetFeaturedPropertiesAsync(int count = 10);
        Task<Property?> GetPropertyWithDetailsAsync(int propertyId);
        Task<IEnumerable<Property>> GetPendingApprovalsAsync();
        Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut);
    }

    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId);
        Task<IEnumerable<Booking>> GetHostBookingsAsync(int hostId);
        Task<IEnumerable<Booking>> GetPropertyBookingsAsync(int propertyId);
        Task<Booking?> GetBookingWithDetailsAsync(int bookingId);
        Task<IEnumerable<Booking>> GetBookingsByStatusAsync(string status);
        Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Booking?> GetByReferenceAsync(string bookingReference);
        Task<bool> HasConflictingBookingAsync(int propertyId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null);
        Task<IEnumerable<Booking>> GetUpcomingBookingsAsync(int userId);
        Task<IEnumerable<Booking>> GetPastBookingsAsync(int userId);
    }

    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetPropertyReviewsAsync(int propertyId);
        Task<IEnumerable<Review>> GetUserReviewsAsync(int userId);
        Task<IEnumerable<Review>> GetHostReviewsAsync(int hostId);
        Task<Review?> GetReviewByBookingAsync(int bookingId);
        Task<bool> CanUserReviewAsync(int userId, int bookingId);
        Task<decimal> GetPropertyAverageRatingAsync(int propertyId);
        Task<IEnumerable<Review>> GetPendingReviewsAsync();
    }

    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetUserPaymentsAsync(int userId);
        Task<IEnumerable<Payment>> GetBookingPaymentsAsync(int bookingId);
        Task<Payment?> GetByTransactionIdAsync(string transactionId);
        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status);
        Task<decimal> GetTotalCommissionAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetHostEarningsAsync(int hostId, DateTime? startDate = null, DateTime? endDate = null);
    }

    public interface IOfferRepository : IRepository<Offer>
    {
        Task<IEnumerable<Offer>> GetActiveOffersAsync();
        Task<IEnumerable<Offer>> GetPropertyOffersAsync(int propertyId);
        Task<Offer?> GetByOfferCodeAsync(string offerCode);
        Task<IEnumerable<Offer>> GetOffersByCreatorAsync(int creatorId);
        Task<bool> IsOfferValidAsync(int offerId, decimal amount);
        Task<IEnumerable<Offer>> GetExpiredOffersAsync();
    }

    public interface IAmenityRepository : IRepository<Amenity>
    {
        Task<IEnumerable<Amenity>> GetActiveAmenitiesAsync();
        Task<IEnumerable<Amenity>> GetAmenitiesByCategoryAsync(string category);
        Task<IEnumerable<Amenity>> GetPropertyAmenitiesAsync(int propertyId);
    }

    public interface IPropertyImageRepository : IRepository<PropertyImage>
    {
        Task<IEnumerable<PropertyImage>> GetPropertyImagesAsync(int propertyId);
        Task<PropertyImage?> GetPrimaryImageAsync(int propertyId);
        Task DeletePropertyImagesAsync(int propertyId);
    }

    public interface ICartItemRepository : IRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetUserCartItemsAsync(int userId);
        Task<CartItem?> GetCartItemAsync(int userId, int propertyId);
        Task DeleteUserCartItemsAsync(int userId);
        Task<int> GetUserCartCountAsync(int userId);
    }

    public interface IPropertyAmenityRepository : IRepository<PropertyAmenity>
    {
        Task DeletePropertyAmenitiesAsync(int propertyId);
        Task<IEnumerable<PropertyAmenity>> GetByPropertyIdAsync(int propertyId);
    }
}