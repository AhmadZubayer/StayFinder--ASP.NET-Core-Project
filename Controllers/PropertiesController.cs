using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StayFinderAPI.Data;
using StayFinderAPI.Models;
using StayFinderAPI.Models.DTOs;
using System.Text.Json;

namespace StayFinderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PropertiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetProperties(
            [FromQuery] string? location = null,
            [FromQuery] string? propertyType = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? guests = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
        {
            var query = _context.Properties
                .Include(p => p.Images)
                .Where(p => p.IsActive);

            // Apply filters
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(p => p.Location.Contains(location));
            }

            if (!string.IsNullOrEmpty(propertyType))
            {
                query = query.Where(p => p.PropertyType == propertyType);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.PricePerNight >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.PricePerNight <= maxPrice.Value);
            }

            if (guests.HasValue)
            {
                query = query.Where(p => p.MaxGuests >= guests.Value);
            }

            // Pagination
            var totalCount = await query.CountAsync();
            var properties = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var propertyDtos = properties.Select(MapToDto).ToList();

            return Ok(new
            {
                Properties = propertyDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyDto>> GetProperty(int id)
        {
            var property = await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (property == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(property));
        }

        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetFeaturedProperties(int count = 8)
        {
            var properties = await _context.Properties
                .Include(p => p.Images)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Rating)
                .ThenByDescending(p => p.ReviewCount)
                .Take(count)
                .ToListAsync();

            var propertyDtos = properties.Select(MapToDto).ToList();
            return Ok(propertyDtos);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> SearchProperties(
            [FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required");
            }

            var properties = await _context.Properties
                .Include(p => p.Images)
                .Where(p => p.IsActive && 
                    (p.Title.Contains(searchTerm) || 
                     p.Location.Contains(searchTerm) || 
                     (p.Description != null && p.Description.Contains(searchTerm))))
                .OrderByDescending(p => p.Rating)
                .Take(20)
                .ToListAsync();

            var propertyDtos = properties.Select(MapToDto).ToList();
            return Ok(propertyDtos);
        }

        private PropertyDto MapToDto(Property property)
        {
            var amenities = new List<string>();
            
            // Safe null check for Amenities
            if (!string.IsNullOrEmpty(property.Amenities))
            {
                try
                {
                    amenities = JsonSerializer.Deserialize<List<string>>(property.Amenities) ?? new List<string>();
                }
                catch (JsonException)
                {
                    // If JSON parsing fails, try splitting by comma
                    amenities = property.Amenities.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim()).ToList();
                }
                catch (Exception)
                {
                    // If all else fails, empty list
                    amenities = new List<string>();
                }
            }

            // Safe image handling
            var primaryImage = property.Images?.FirstOrDefault(i => i.IsPrimary) 
                ?? property.Images?.OrderBy(i => i.DisplayOrder).FirstOrDefault();

            // Determine badge - safe null checks
            var badgeText = "";
            var badgeClass = "";
            
            if (property.IsSuperhost)
            {
                badgeText = "Luxury";
                badgeClass = "badge-primary";
            }
            else if (property.IsNew)
            {
                badgeText = "New";
                badgeClass = "badge-secondary";
            }
            else if (property.IsPopular)
            {
                badgeText = "Popular";
                badgeClass = "badge-accent";
            }
            else if (property.IsLuxury)
            {
                badgeText = "Premium";
                badgeClass = "badge-info";
            }

            return new PropertyDto
            {
                Id = property.Id,
                Title = property.Title ?? "",
                Description = property.Description ?? "",
                Location = property.Location ?? "",
                PricePerNight = property.PricePerNight,
                Rating = property.Rating,
                ReviewCount = property.ReviewCount,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                MaxGuests = property.MaxGuests,
                PropertyType = property.PropertyType ?? "",
                Amenities = amenities,
                IsSuperhost = property.IsSuperhost,
                IsNew = property.IsNew,
                IsPopular = property.IsPopular,
                IsLuxury = property.IsLuxury,
                PrimaryImageUrl = primaryImage?.ImageUrl ?? "https://via.placeholder.com/400x300?text=Property+Image",
                BadgeText = badgeText,
                BadgeClass = badgeClass,
                Images = property.Images?.Select(i => new PropertyImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl ?? "",
                    AltText = i.AltText ?? "",
                    IsPrimary = i.IsPrimary,
                    DisplayOrder = i.DisplayOrder
                }).OrderBy(i => i.DisplayOrder).ToList() ?? new List<PropertyImageDto>()
            };
        }
    }
}