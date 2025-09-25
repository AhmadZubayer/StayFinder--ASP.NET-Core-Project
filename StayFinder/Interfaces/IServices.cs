using StayFinder.Models;

namespace StayFinder.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
        Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
        Task LogoutAsync(int userId);
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        bool ValidateRefreshToken(string refreshToken);
    }

    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetUserByIdAsync(int id);
        Task<ApiResponse<UserDto>> GetUserByEmailAsync(string email);
        Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync();
        Task<ApiResponse<IEnumerable<UserDto>>> GetUsersByRoleAsync(string role);
        Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UserCreateDto userDto);
        Task<ApiResponse<bool>> DeleteUserAsync(int id);
        Task<ApiResponse<bool>> ActivateUserAsync(int id);
        Task<ApiResponse<bool>> DeactivateUserAsync(int id);
        Task<ApiResponse<bool>> ApproveHostAsync(int hostId);
        Task<ApiResponse<bool>> RejectHostAsync(int hostId);
        Task<ApiResponse<IEnumerable<UserDto>>> GetPendingHostsAsync();
        Task<ApiResponse<UserDto>> GetUserProfileAsync(int userId);
        Task<ApiResponse<UserDto>> UpdateUserProfileAsync(int userId, UserCreateDto userDto);
    }

    public interface IPropertyService
    {
        Task<ApiResponse<PropertyDto>> GetPropertyByIdAsync(int id);
        Task<ApiResponse<PropertyDto>> GetPropertyWithDetailsAsync(int id);
        Task<ApiResponse<IEnumerable<PropertyDto>>> GetAllPropertiesAsync();
        Task<ApiResponse<IEnumerable<PropertyDto>>> GetHostPropertiesAsync(int hostId);
        Task<ApiResponse<PagedResult<PropertyDto>>> SearchPropertiesAsync(PropertySearchDto searchDto);
        Task<ApiResponse<PropertyDto>> CreatePropertyAsync(int hostId, PropertyCreateDto propertyDto);
        Task<ApiResponse<PropertyDto>> UpdatePropertyAsync(int id, int hostId, PropertyUpdateDto propertyDto);
        Task<ApiResponse<bool>> DeletePropertyAsync(int id, int hostId);
        Task<ApiResponse<bool>> ApprovePropertyAsync(int id);
        Task<ApiResponse<bool>> RejectPropertyAsync(int id, string reason);
        Task<ApiResponse<IEnumerable<PropertyDto>>> GetPendingApprovalsAsync();
        Task<ApiResponse<IEnumerable<PropertyDto>>> GetFeaturedPropertiesAsync(int count = 10);
        Task<ApiResponse<bool>> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut);
        Task<ApiResponse<IEnumerable<PropertyDto>>> GetAvailablePropertiesAsync(DateTime checkIn, DateTime checkOut);
    }

    public interface IBookingService
    {
        Task<ApiResponse<BookingDto>> GetBookingByIdAsync(int id);
        Task<ApiResponse<BookingDto>> GetBookingByReferenceAsync(string reference);
        Task<ApiResponse<IEnumerable<BookingDto>>> GetUserBookingsAsync(int userId);
        Task<ApiResponse<IEnumerable<BookingDto>>> GetHostBookingsAsync(int hostId);
        Task<ApiResponse<IEnumerable<BookingDto>>> GetPropertyBookingsAsync(int propertyId);
        Task<ApiResponse<BookingDto>> CreateBookingAsync(int userId, BookingCreateDto bookingDto);
        Task<ApiResponse<BookingDto>> UpdateBookingAsync(int id, int userId, BookingUpdateDto bookingDto);
        Task<ApiResponse<bool>> CancelBookingAsync(int id, int userId);
        Task<ApiResponse<bool>> ConfirmBookingAsync(int id, int hostId);
        Task<ApiResponse<IEnumerable<BookingDto>>> GetUpcomingBookingsAsync(int userId);
        Task<ApiResponse<IEnumerable<BookingDto>>> GetPastBookingsAsync(int userId);
        Task<ApiResponse<decimal>> CalculateBookingTotalAsync(BookingCreateDto bookingDto);
        string GenerateBookingReference();
    }

    public interface IReviewService
    {
        Task<ApiResponse<ReviewDto>> GetReviewByIdAsync(int id);
        Task<ApiResponse<IEnumerable<ReviewDto>>> GetPropertyReviewsAsync(int propertyId);
        Task<ApiResponse<IEnumerable<ReviewDto>>> GetUserReviewsAsync(int userId);
        Task<ApiResponse<IEnumerable<ReviewDto>>> GetHostReviewsAsync(int hostId);
        Task<ApiResponse<ReviewDto>> CreateReviewAsync(int userId, ReviewCreateDto reviewDto);
        Task<ApiResponse<ReviewDto>> UpdateReviewAsync(int id, int userId, ReviewUpdateDto reviewDto);
        Task<ApiResponse<bool>> DeleteReviewAsync(int id, int userId);
        Task<ApiResponse<bool>> ApproveReviewAsync(int id);
        Task<ApiResponse<bool>> RejectReviewAsync(int id);
        Task<ApiResponse<bool>> CanUserReviewAsync(int userId, int bookingId);
        Task<ApiResponse<decimal>> GetPropertyAverageRatingAsync(int propertyId);
    }

    public interface IPaymentService
    {
        Task<ApiResponse<PaymentDto>> GetPaymentByIdAsync(int id);
        Task<ApiResponse<IEnumerable<PaymentDto>>> GetUserPaymentsAsync(int userId);
        Task<ApiResponse<IEnumerable<PaymentDto>>> GetBookingPaymentsAsync(int bookingId);
        Task<ApiResponse<PaymentDto>> CreatePaymentAsync(int userId, PaymentCreateDto paymentDto);
        Task<ApiResponse<PaymentDto>> UpdatePaymentStatusAsync(int id, PaymentUpdateDto paymentDto);
        Task<ApiResponse<PaymentDto>> ProcessPaymentAsync(int bookingId, PaymentCreateDto paymentDto);
        Task<ApiResponse<PaymentDto>> RefundPaymentAsync(int paymentId, decimal refundAmount);
        Task<ApiResponse<decimal>> GetTotalCommissionAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<ApiResponse<decimal>> GetHostEarningsAsync(int hostId, DateTime? startDate = null, DateTime? endDate = null);
    }

    public interface IOfferService
    {
        Task<ApiResponse<OfferDto>> GetOfferByIdAsync(int id);
        Task<ApiResponse<IEnumerable<OfferDto>>> GetActiveOffersAsync();
        Task<ApiResponse<IEnumerable<OfferDto>>> GetPropertyOffersAsync(int propertyId);
        Task<ApiResponse<OfferDto>> GetOfferByCodeAsync(string offerCode);
        Task<ApiResponse<OfferDto>> CreateOfferAsync(int creatorId, OfferCreateDto offerDto);
        Task<ApiResponse<OfferDto>> UpdateOfferAsync(int id, int creatorId, OfferUpdateDto offerDto);
        Task<ApiResponse<bool>> DeleteOfferAsync(int id, int creatorId);
        Task<ApiResponse<bool>> DeactivateExpiredOffersAsync();
        Task<ApiResponse<decimal>> CalculateDiscountAsync(int offerId, decimal amount);
        Task<ApiResponse<bool>> ValidateOfferAsync(int offerId, decimal amount);
    }

    public interface IAmenityService
    {
        Task<ApiResponse<IEnumerable<AmenityDto>>> GetAllAmenitiesAsync();
        Task<ApiResponse<IEnumerable<AmenityDto>>> GetActiveAmenitiesAsync();
        Task<ApiResponse<IEnumerable<AmenityDto>>> GetAmenitiesByCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<AmenityDto>>> GetPropertyAmenitiesAsync(int propertyId);
        Task<ApiResponse<AmenityDto>> CreateAmenityAsync(AmenityCreateDto amenityDto);
        Task<ApiResponse<AmenityDto>> UpdateAmenityAsync(int id, AmenityCreateDto amenityDto);
        Task<ApiResponse<bool>> DeleteAmenityAsync(int id);
    }

    public interface ICartService
    {
        Task<ApiResponse<IEnumerable<CartItemDto>>> GetUserCartAsync(int userId);
        Task<ApiResponse<CartItemDto>> AddToCartAsync(int userId, CartItemCreateDto cartItemDto);
        Task<ApiResponse<CartItemDto>> UpdateCartItemAsync(int userId, int cartItemId, CartItemUpdateDto cartItemDto);
        Task<ApiResponse<bool>> RemoveFromCartAsync(int userId, int cartItemId);
        Task<ApiResponse<bool>> ClearCartAsync(int userId);
        Task<ApiResponse<int>> GetCartCountAsync(int userId);
        Task<ApiResponse<decimal>> GetCartTotalAsync(int userId);
    }

    public interface IAnalyticsService
    {
        Task<ApiResponse<object>> GetDashboardStatsAsync(int? userId = null, string? role = null);
        Task<ApiResponse<object>> GetBookingStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<ApiResponse<object>> GetRevenueStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<ApiResponse<object>> GetUserStatsAsync();
        Task<ApiResponse<object>> GetPropertyStatsAsync();
        Task<ApiResponse<object>> GetHostPerformanceAsync(int hostId, DateTime? startDate = null, DateTime? endDate = null);
    }

    public interface IFileService
    {
        Task<ApiResponse<string>> UploadImageAsync(IFormFile file, string folder = "images");
        Task<ApiResponse<IEnumerable<string>>> UploadMultipleImagesAsync(IEnumerable<IFormFile> files, string folder = "images");
        Task<ApiResponse<bool>> DeleteImageAsync(string imagePath);
        string GetImageUrl(string imagePath);
        bool IsValidImageFile(IFormFile file);
    }
}