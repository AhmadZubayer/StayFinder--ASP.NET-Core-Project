using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StayFinder.Data;
using StayFinder.Models;

namespace StayFinder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly StayFinderDbContext _context;
        private readonly ILogger<PropertiesController> _logger;

        public PropertiesController(StayFinderDbContext context, ILogger<PropertiesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Properties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetProperties()
        {
            try
            {
                var properties = await _context.Properties
                    .Select(p => new PropertyDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Location = p.Location,
                        City = p.City,
                        Description = p.Description,
                        PropertyType = p.PropertyType,
                        Rating = p.Rating,
                        Price = p.Price,
                        AvailableFrom = p.AvailableFrom,
                        AvailableTo = p.AvailableTo,
                        ImagePath = p.ImagePath
                    })
                    .ToListAsync();

                return Ok(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving properties");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        // GET: api/Properties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyDto>> GetProperty(int id)
        {
            try
            {
                var property = await _context.Properties.FindAsync(id);

                if (property == null)
                {
                    return NotFound();
                }

                var propertyDto = new PropertyDto
                {
                    Id = property.Id,
                    Title = property.Title,
                    Location = property.Location,
                    City = property.City,
                    Description = property.Description,
                    PropertyType = property.PropertyType,
                    Rating = property.Rating,
                    Price = property.Price,
                    AvailableFrom = property.AvailableFrom,
                    AvailableTo = property.AvailableTo,
                    ImagePath = property.ImagePath
                };

                return Ok(propertyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving property with id {PropertyId}", id);
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        // GET: api/Properties/filter
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> FilterProperties(
            [FromQuery] string? city = null,
            [FromQuery] string? type = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] decimal? minRating = null)
        {
            try
            {
                var query = _context.Properties.AsQueryable();

                if (!string.IsNullOrEmpty(city))
                {
                    query = query.Where(p => p.City.ToLower().Contains(city.ToLower()));
                }

                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(p => p.PropertyType.ToLower() == type.ToLower());
                }

                if (minPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= maxPrice.Value);
                }

                if (minRating.HasValue)
                {
                    query = query.Where(p => p.Rating >= minRating.Value);
                }

                var properties = await query
                    .Select(p => new PropertyDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Location = p.Location,
                        City = p.City,
                        Description = p.Description,
                        PropertyType = p.PropertyType,
                        Rating = p.Rating,
                        Price = p.Price,
                        AvailableFrom = p.AvailableFrom,
                        AvailableTo = p.AvailableTo,
                        ImagePath = p.ImagePath
                    })
                    .ToListAsync();

                return Ok(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering properties");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        // GET: api/Properties/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> SearchProperties(
            [FromQuery] string? q = null,
            [FromQuery] string? city = null,
            [FromQuery] string? type = null)
        {
            try
            {
                var query = _context.Properties.AsQueryable();

                if (!string.IsNullOrEmpty(q))
                {
                    query = query.Where(p => 
                        p.Title.ToLower().Contains(q.ToLower()) ||
                        p.Description.ToLower().Contains(q.ToLower()) ||
                        p.Location.ToLower().Contains(q.ToLower()) ||
                        p.City.ToLower().Contains(q.ToLower()));
                }

                if (!string.IsNullOrEmpty(city))
                {
                    query = query.Where(p => p.City.ToLower().Contains(city.ToLower()));
                }

                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(p => p.PropertyType.ToLower() == type.ToLower());
                }

                var properties = await query
                    .Select(p => new PropertyDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Location = p.Location,
                        City = p.City,
                        Description = p.Description,
                        PropertyType = p.PropertyType,
                        Rating = p.Rating,
                        Price = p.Price,
                        AvailableFrom = p.AvailableFrom,
                        AvailableTo = p.AvailableTo,
                        ImagePath = p.ImagePath
                    })
                    .ToListAsync();

                return Ok(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching properties");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }
    }
}