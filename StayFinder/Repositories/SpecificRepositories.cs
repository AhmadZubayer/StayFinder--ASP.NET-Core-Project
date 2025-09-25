using Microsoft.EntityFrameworkCore;
using StayFinder.Data;
using StayFinder.Interfaces;
using StayFinder.Models;

namespace StayFinder.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(StayFinderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId)
        {
            return await _dbSet
                .Include(b => b.Property)
                    .ThenInclude(p => p.PropertyImages)
                .Include(b => b.Property.Host)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetHostBookingsAsync(int hostId)
        {
            return await _dbSet
                .Include(b => b.Property)
                .Include(b => b.User)
                .Where(b => b.Property.HostId == hostId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetPropertyBookingsAsync(int propertyId)
        {
            return await _dbSet
                .Include(b => b.User)
                .Where(b => b.PropertyId == propertyId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingWithDetailsAsync(int bookingId)
        {
            return await _dbSet
                .Include(b => b.Property)
                    .ThenInclude(p => p.Host)
                .Include(b => b.Property.PropertyImages)
                .Include(b => b.User)
                .Include(b => b.Payments)
                .Include(b => b.Reviews)
                .Include(b => b.Offer)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(string status)
        {
            return await _dbSet
                .Include(b => b.Property)
                .Include(b => b.User)
                .Where(b => b.Status == status)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(b => b.Property)
                .Include(b => b.User)
                .Where(b => b.CheckIn >= startDate && b.CheckOut <= endDate)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Booking?> GetByReferenceAsync(string bookingReference)
        {
            return await _dbSet
                .Include(b => b.Property)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingReference == bookingReference);
        }

        public async Task<bool> HasConflictingBookingAsync(int propertyId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
        {
            var query = _dbSet.Where(b => 
                b.PropertyId == propertyId && 
                b.Status != "Cancelled" &&
                ((b.CheckIn <= checkIn && b.CheckOut > checkIn) ||
                 (b.CheckIn < checkOut && b.CheckOut >= checkOut) ||
                 (b.CheckIn >= checkIn && b.CheckOut <= checkOut)));

            if (excludeBookingId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBookingId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Booking>> GetUpcomingBookingsAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Include(b => b.Property)
                    .ThenInclude(p => p.PropertyImages)
                .Where(b => b.UserId == userId && b.CheckIn >= today && b.Status != "Cancelled")
                .OrderBy(b => b.CheckIn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetPastBookingsAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Include(b => b.Property)
                    .ThenInclude(p => p.PropertyImages)
                .Where(b => b.UserId == userId && b.CheckOut < today)
                .OrderByDescending(b => b.CheckOut)
                .ToListAsync();
        }
    }

    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(StayFinderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetPropertyReviewsAsync(int propertyId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Booking)
                .Where(r => r.PropertyId == propertyId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetUserReviewsAsync(int userId)
        {
            return await _dbSet
                .Include(r => r.Property)
                .Include(r => r.Booking)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetHostReviewsAsync(int hostId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Property)
                .Include(r => r.Booking)
                .Where(r => r.HostId == hostId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewByBookingAsync(int bookingId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Property)
                .FirstOrDefaultAsync(r => r.BookingId == bookingId);
        }

        public async Task<bool> CanUserReviewAsync(int userId, int bookingId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null || booking.Status == "Cancelled" || booking.CheckOut > DateTime.UtcNow)
                return false;

            var existingReview = await _dbSet
                .AnyAsync(r => r.BookingId == bookingId);

            return !existingReview;
        }

        public async Task<decimal> GetPropertyAverageRatingAsync(int propertyId)
        {
            var reviews = await _dbSet
                .Where(r => r.PropertyId == propertyId && r.IsApproved)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return (decimal)reviews.Average(r => r.Rating);
        }

        public async Task<IEnumerable<Review>> GetPendingReviewsAsync()
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Property)
                .Where(r => !r.IsApproved)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }
    }

    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(StayFinderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Payment>> GetUserPaymentsAsync(int userId)
        {
            return await _dbSet
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Property)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetBookingPaymentsAsync(int bookingId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => p.BookingId == bookingId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await _dbSet
                .Include(p => p.Booking)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status)
        {
            return await _dbSet
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Property)
                .Include(p => p.User)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalCommissionAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.Where(p => p.Status == "Completed");

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate.Value);

            return await query.SumAsync(p => p.CommissionAmount);
        }

        public async Task<decimal> GetHostEarningsAsync(int hostId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Property)
                .Where(p => p.Status == "Completed" && p.Booking.Property.HostId == hostId);

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate.Value);

            return await query.SumAsync(p => p.HostAmount);
        }
    }

    public class OfferRepository : Repository<Offer>, IOfferRepository
    {
        public OfferRepository(StayFinderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Offer>> GetActiveOffersAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(o => o.Property)
                .Include(o => o.CreatedByUser)
                .Where(o => o.IsActive && o.ValidFrom <= now && o.ValidTo >= now)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Offer>> GetPropertyOffersAsync(int propertyId)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(o => o.PropertyId == propertyId && o.IsActive && o.ValidFrom <= now && o.ValidTo >= now)
                .OrderByDescending(o => o.DiscountValue)
                .ToListAsync();
        }

        public async Task<Offer?> GetByOfferCodeAsync(string offerCode)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(o => o.Property)
                .FirstOrDefaultAsync(o => 
                    o.OfferCode == offerCode && 
                    o.IsActive && 
                    o.ValidFrom <= now && 
                    o.ValidTo >= now);
        }

        public async Task<IEnumerable<Offer>> GetOffersByCreatorAsync(int creatorId)
        {
            return await _dbSet
                .Include(o => o.Property)
                .Where(o => o.CreatedBy == creatorId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsOfferValidAsync(int offerId, decimal amount)
        {
            var offer = await _dbSet.FindAsync(offerId);

            if (offer == null || !offer.IsActive)
                return false;

            var now = DateTime.UtcNow;
            if (offer.ValidFrom > now || offer.ValidTo < now)
                return false;

            if (offer.MinimumAmount.HasValue && amount < offer.MinimumAmount.Value)
                return false;

            if (offer.UsageLimit.HasValue && offer.UsedCount >= offer.UsageLimit.Value)
                return false;

            return true;
        }

        public async Task<IEnumerable<Offer>> GetExpiredOffersAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(o => o.ValidTo < now && o.IsActive)
                .ToListAsync();
        }
    }

    public class AmenityRepository : Repository<Amenity>, IAmenityRepository
    {
        public AmenityRepository(StayFinderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Amenity>> GetActiveAmenitiesAsync()
        {
            return await _dbSet
                .Where(a => a.IsActive)
                .OrderBy(a => a.Category)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Amenity>> GetAmenitiesByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(a => a.Category == category && a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Amenity>> GetPropertyAmenitiesAsync(int propertyId)
        {
            return await _context.PropertyAmenities
                .Include(pa => pa.Amenity)
                .Where(pa => pa.PropertyId == propertyId && pa.Amenity.IsActive)
                .Select(pa => pa.Amenity)
                .ToListAsync();
        }
    }

    public class PropertyImageRepository : Repository<PropertyImage>, IPropertyImageRepository
    {
        public PropertyImageRepository(StayFinderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PropertyImage>> GetPropertyImagesAsync(int propertyId)
        {
            return await _dbSet
                .Where(pi => pi.PropertyId == propertyId)
                .OrderBy(pi => pi.DisplayOrder)
                .ThenBy(pi => pi.Id)
                .ToListAsync();
        }

        public async Task<PropertyImage?> GetPrimaryImageAsync(int propertyId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(pi => pi.PropertyId == propertyId && pi.IsPrimary);
        }

        public async Task DeletePropertyImagesAsync(int propertyId)
        {
            var images = await _dbSet
                .Where(pi => pi.PropertyId == propertyId)
                .ToListAsync();

            _dbSet.RemoveRange(images);
            await _context.SaveChangesAsync();
        }
    }

    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(StayFinderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CartItem>> GetUserCartItemsAsync(int userId)
        {
            return await _dbSet
                .Include(ci => ci.Property)
                    .ThenInclude(p => p.PropertyImages)
                .Where(ci => ci.UserId == userId)
                .OrderByDescending(ci => ci.CreatedAt)
                .ToListAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(int userId, int propertyId)
        {
            return await _dbSet
                .Include(ci => ci.Property)
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.PropertyId == propertyId);
        }

        public async Task DeleteUserCartItemsAsync(int userId)
        {
            var cartItems = await _dbSet
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            _dbSet.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUserCartCountAsync(int userId)
        {
            return await _dbSet.CountAsync(ci => ci.UserId == userId);
        }
    }

    public class PropertyAmenityRepository : Repository<PropertyAmenity>, IPropertyAmenityRepository
    {
        public PropertyAmenityRepository(StayFinderDbContext context) : base(context)
        {
        }

        public async Task DeletePropertyAmenitiesAsync(int propertyId)
        {
            var propertyAmenities = await _dbSet
                .Where(pa => pa.PropertyId == propertyId)
                .ToListAsync();

            _dbSet.RemoveRange(propertyAmenities);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PropertyAmenity>> GetByPropertyIdAsync(int propertyId)
        {
            return await _dbSet
                .Include(pa => pa.Amenity)
                .Where(pa => pa.PropertyId == propertyId)
                .ToListAsync();
        }
    }
}