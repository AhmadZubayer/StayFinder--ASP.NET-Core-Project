using Microsoft.EntityFrameworkCore;
using StayFinder.Data;
using StayFinder.Interfaces;
using StayFinder.Models;

namespace StayFinder.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(StayFinderDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _dbSet
                .Where(u => u.Role == role && u.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetHostsAsync()
        {
            return await _dbSet
                .Where(u => u.Role == "host" && u.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet
                .Where(u => u.IsActive)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithBookingsAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Property)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithPropertiesAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Properties)
                    .ThenInclude(p => p.PropertyImages)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }

    public class PropertyRepository : Repository<Property>, IPropertyRepository
    {
        public PropertyRepository(StayFinderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Property>> GetByHostIdAsync(int hostId)
        {
            return await _dbSet
                .Include(p => p.PropertyImages)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .Where(p => p.HostId == hostId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> SearchPropertiesAsync(PropertySearchDto searchDto)
        {
            var query = _dbSet
                .Include(p => p.Host)
                .Include(p => p.PropertyImages)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive && p.IsApproved);

            // Apply filters
            if (!string.IsNullOrEmpty(searchDto.Location))
            {
                query = query.Where(p => p.Location.Contains(searchDto.Location) || p.City.Contains(searchDto.Location));
            }

            if (!string.IsNullOrEmpty(searchDto.City))
            {
                query = query.Where(p => p.City.Contains(searchDto.City));
            }

            if (!string.IsNullOrEmpty(searchDto.PropertyType))
            {
                query = query.Where(p => p.PropertyType == searchDto.PropertyType);
            }

            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= searchDto.MinPrice.Value);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= searchDto.MaxPrice.Value);
            }

            if (searchDto.Guests.HasValue)
            {
                query = query.Where(p => p.MaxGuests >= searchDto.Guests.Value);
            }

            if (searchDto.Bedrooms.HasValue)
            {
                query = query.Where(p => p.Bedrooms >= searchDto.Bedrooms.Value);
            }

            if (searchDto.Bathrooms.HasValue)
            {
                query = query.Where(p => p.Bathrooms >= searchDto.Bathrooms.Value);
            }

            if (searchDto.MinRating.HasValue)
            {
                query = query.Where(p => p.Rating >= searchDto.MinRating.Value);
            }

            if (searchDto.IsInstantBook.HasValue)
            {
                query = query.Where(p => p.IsInstantBook == searchDto.IsInstantBook.Value);
            }

            // Availability filter
            if (searchDto.CheckIn.HasValue && searchDto.CheckOut.HasValue)
            {
                var checkIn = searchDto.CheckIn.Value;
                var checkOut = searchDto.CheckOut.Value;
                
                query = query.Where(p => 
                    p.AvailableFrom <= checkIn && 
                    p.AvailableTo >= checkOut &&
                    !p.Bookings.Any(b => 
                        b.Status != "Cancelled" &&
                        ((b.CheckIn <= checkIn && b.CheckOut > checkIn) ||
                         (b.CheckIn < checkOut && b.CheckOut >= checkOut) ||
                         (b.CheckIn >= checkIn && b.CheckOut <= checkOut))));
            }

            // Amenity filter
            if (searchDto.AmenityIds.Any())
            {
                query = query.Where(p => 
                    searchDto.AmenityIds.All(aid => 
                        p.PropertyAmenities.Any(pa => pa.AmenityId == aid)));
            }

            // Apply sorting
            switch (searchDto.SortBy.ToLower())
            {
                case "price":
                    query = searchDto.SortOrder.ToLower() == "desc" 
                        ? query.OrderByDescending(p => p.Price)
                        : query.OrderBy(p => p.Price);
                    break;
                case "rating":
                    query = searchDto.SortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(p => p.Rating ?? 0)
                        : query.OrderBy(p => p.Rating ?? 0);
                    break;
                case "created":
                    query = searchDto.SortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(p => p.CreatedAt)
                        : query.OrderBy(p => p.CreatedAt);
                    break;
                default:
                    query = query.OrderBy(p => p.Price);
                    break;
            }

            // Apply pagination
            return await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetSearchResultsCountAsync(PropertySearchDto searchDto)
        {
            var query = _dbSet.Where(p => p.IsActive && p.IsApproved);

            // Apply the same filters as SearchPropertiesAsync but without includes for performance
            if (!string.IsNullOrEmpty(searchDto.Location))
            {
                query = query.Where(p => p.Location.Contains(searchDto.Location) || p.City.Contains(searchDto.Location));
            }

            if (!string.IsNullOrEmpty(searchDto.City))
            {
                query = query.Where(p => p.City.Contains(searchDto.City));
            }

            if (!string.IsNullOrEmpty(searchDto.PropertyType))
            {
                query = query.Where(p => p.PropertyType == searchDto.PropertyType);
            }

            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= searchDto.MinPrice.Value);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= searchDto.MaxPrice.Value);
            }

            if (searchDto.Guests.HasValue)
            {
                query = query.Where(p => p.MaxGuests >= searchDto.Guests.Value);
            }

            if (searchDto.Bedrooms.HasValue)
            {
                query = query.Where(p => p.Bedrooms >= searchDto.Bedrooms.Value);
            }

            if (searchDto.Bathrooms.HasValue)
            {
                query = query.Where(p => p.Bathrooms >= searchDto.Bathrooms.Value);
            }

            if (searchDto.MinRating.HasValue)
            {
                query = query.Where(p => p.Rating >= searchDto.MinRating.Value);
            }

            if (searchDto.IsInstantBook.HasValue)
            {
                query = query.Where(p => p.IsInstantBook == searchDto.IsInstantBook.Value);
            }

            // Availability filter
            if (searchDto.CheckIn.HasValue && searchDto.CheckOut.HasValue)
            {
                var checkIn = searchDto.CheckIn.Value;
                var checkOut = searchDto.CheckOut.Value;
                
                query = query.Where(p => 
                    p.AvailableFrom <= checkIn && 
                    p.AvailableTo >= checkOut &&
                    !p.Bookings.Any(b => 
                        b.Status != "Cancelled" &&
                        ((b.CheckIn <= checkIn && b.CheckOut > checkIn) ||
                         (b.CheckIn < checkOut && b.CheckOut >= checkOut) ||
                         (b.CheckIn >= checkIn && b.CheckOut <= checkOut))));
            }

            // Amenity filter
            if (searchDto.AmenityIds.Any())
            {
                query = query.Where(p => 
                    searchDto.AmenityIds.All(aid => 
                        p.PropertyAmenities.Any(pa => pa.AmenityId == aid)));
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<Property>> GetAvailablePropertiesAsync(DateTime checkIn, DateTime checkOut)
        {
            return await _dbSet
                .Include(p => p.PropertyImages)
                .Where(p => p.IsActive && p.IsApproved &&
                    p.AvailableFrom <= checkIn && 
                    p.AvailableTo >= checkOut &&
                    !p.Bookings.Any(b => 
                        b.Status != "Cancelled" &&
                        ((b.CheckIn <= checkIn && b.CheckOut > checkIn) ||
                         (b.CheckIn < checkOut && b.CheckOut >= checkOut) ||
                         (b.CheckIn >= checkIn && b.CheckOut <= checkOut))))
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByCityAsync(string city)
        {
            return await _dbSet
                .Include(p => p.PropertyImages)
                .Where(p => p.City.Contains(city) && p.IsActive && p.IsApproved)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetFeaturedPropertiesAsync(int count = 10)
        {
            return await _dbSet
                .Include(p => p.PropertyImages)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive && p.IsApproved)
                .OrderByDescending(p => p.Rating)
                .ThenByDescending(p => p.ReviewCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Property?> GetPropertyWithDetailsAsync(int propertyId)
        {
            return await _dbSet
                .Include(p => p.Host)
                .Include(p => p.PropertyImages)
                .Include(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .Include(p => p.Bookings.Where(b => b.Status != "Cancelled"))
                .FirstOrDefaultAsync(p => p.Id == propertyId);
        }

        public async Task<IEnumerable<Property>> GetPendingApprovalsAsync()
        {
            return await _dbSet
                .Include(p => p.Host)
                .Include(p => p.PropertyImages)
                .Where(p => !p.IsApproved)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut)
        {
            var property = await _dbSet
                .Include(p => p.Bookings)
                .FirstOrDefaultAsync(p => p.Id == propertyId);

            if (property == null || !property.IsActive || !property.IsApproved)
                return false;

            if (property.AvailableFrom > checkIn || property.AvailableTo < checkOut)
                return false;

            return !property.Bookings.Any(b =>
                b.Status != "Cancelled" &&
                ((b.CheckIn <= checkIn && b.CheckOut > checkIn) ||
                 (b.CheckIn < checkOut && b.CheckOut >= checkOut) ||
                 (b.CheckIn >= checkIn && b.CheckOut <= checkOut)));
        }
    }
}