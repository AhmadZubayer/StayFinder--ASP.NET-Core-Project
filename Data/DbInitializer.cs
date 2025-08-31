using StayFinderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace StayFinderAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if data already exists
            if (context.Properties.Any())
            {
                return; // Database has been seeded
            }

            var properties = new Property[]
            {
                new Property
                {
                    Title = "Modern Downtown Apartment",
                    Description = "A beautiful modern apartment in the heart of downtown with stunning city views.",
                    Location = "New York, NY",
                    PricePerNight = 120.00m,
                    Rating = 4.9m,
                    ReviewCount = 156,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    MaxGuests = 4,
                    PropertyType = "Apartment",
                    Amenities = "[\"WiFi\", \"Kitchen\", \"Air conditioning\", \"Heating\"]",
                    IsSuperhost = true
                },
                new Property
                {
                    Title = "Luxury Hotel Suite",
                    Description = "Experience luxury at its finest with ocean views and premium amenities.",
                    Location = "Los Angeles, CA",
                    PricePerNight = 250.00m,
                    Rating = 4.8m,
                    ReviewCount = 89,
                    Bedrooms = 1,
                    Bathrooms = 1,
                    MaxGuests = 2,
                    PropertyType = "Hotel",
                    Amenities = "[\"Ocean view\", \"Pool\", \"Spa\", \"Room service\"]",
                    IsLuxury = true
                },
                new Property
                {
                    Title = "Cozy Mountain Cabin",
                    Description = "Escape to nature in this charming cabin with mountain views and modern amenities.",
                    Location = "Aspen, CO",
                    PricePerNight = 180.00m,
                    Rating = 4.7m,
                    ReviewCount = 234,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    MaxGuests = 6,
                    PropertyType = "Cabin",
                    Amenities = "[\"Fireplace\", \"Hot tub\", \"Mountain view\", \"Kitchen\"]",
                    IsNew = true
                },
                new Property
                {
                    Title = "Beachfront Villa",
                    Description = "Wake up to ocean waves in this stunning beachfront villa with private beach access.",
                    Location = "Miami, FL",
                    PricePerNight = 350.00m,
                    Rating = 5.0m,
                    ReviewCount = 67,
                    Bedrooms = 4,
                    Bathrooms = 3,
                    MaxGuests = 8,
                    PropertyType = "Villa",
                    Amenities = "[\"Beach access\", \"Pool\", \"Ocean view\", \"BBQ grill\"]",
                    IsPopular = true,
                    IsLuxury = true
                },
                new Property
                {
                    Title = "Industrial Loft",
                    Description = "Stylish loft in converted warehouse with high ceilings and modern design.",
                    Location = "Chicago, IL",
                    PricePerNight = 95.00m,
                    Rating = 4.6m,
                    ReviewCount = 145,
                    Bedrooms = 1,
                    Bathrooms = 1,
                    MaxGuests = 2,
                    PropertyType = "Loft",
                    Amenities = "[\"High ceilings\", \"Gym access\", \"WiFi\", \"Kitchen\"]"
                },
                new Property
                {
                    Title = "Boutique Hotel Room",
                    Description = "Elegant boutique hotel room with personalized service and city views.",
                    Location = "San Francisco, CA",
                    PricePerNight = 200.00m,
                    Rating = 4.9m,
                    ReviewCount = 198,
                    Bedrooms = 1,
                    Bathrooms = 1,
                    MaxGuests = 2,
                    PropertyType = "Hotel",
                    Amenities = "[\"City view\", \"Spa access\", \"Concierge\", \"WiFi\"]",
                    IsLuxury = true
                },
                new Property
                {
                    Title = "Magical Tree House",
                    Description = "Unique tree house experience surrounded by nature and tranquility.",
                    Location = "Portland, OR",
                    PricePerNight = 140.00m,
                    Rating = 4.8m,
                    ReviewCount = 87,
                    Bedrooms = 1,
                    Bathrooms = 1,
                    MaxGuests = 2,
                    PropertyType = "Unique",
                    Amenities = "[\"Tree top\", \"Nature view\", \"Unique experience\"]"
                },
                new Property
                {
                    Title = "Cozy Studio",
                    Description = "Compact and efficient studio apartment perfect for solo travelers or couples.",
                    Location = "Boston, MA",
                    PricePerNight = 85.00m,
                    Rating = 4.5m,
                    ReviewCount = 123,
                    Bedrooms = 0,
                    Bathrooms = 1,
                    MaxGuests = 2,
                    PropertyType = "Studio",
                    Amenities = "[\"Kitchenette\", \"Metro access\", \"WiFi\"]"
                }
            };

            context.Properties.AddRange(properties);
            context.SaveChanges();

            // Add sample images
            var images = new PropertyImage[]
            {
                new PropertyImage
                {
                    PropertyId = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1564013799919-ab600027ffc6?w=400&h=300&fit=crop",
                    AltText = "Modern Apartment",
                    IsPrimary = true,
                    DisplayOrder = 1
                },
                new PropertyImage
                {
                    PropertyId = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=400&h=300&fit=crop",
                    AltText = "Luxury Hotel",
                    IsPrimary = true,
                    DisplayOrder = 1
                },
                new PropertyImage
                {
                    PropertyId = 3,
                    ImageUrl = "https://images.unsplash.com/photo-1449824913935-59a10b8d2000?w=400&h=300&fit=crop",
                    AltText = "Cozy Cabin",
                    IsPrimary = true,
                    DisplayOrder = 1
                },
                new PropertyImage
                {
                    PropertyId = 4,
                    ImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=400&h=300&fit=crop",
                    AltText = "Beach House",
                    IsPrimary = true,
                    DisplayOrder = 1
                },
                new PropertyImage
                {
                    PropertyId = 5,
                    ImageUrl = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=400&h=300&fit=crop",
                    AltText = "Urban Loft",
                    IsPrimary = true,
                    DisplayOrder = 1
                },
                new PropertyImage
                {
                    PropertyId = 6,
                    ImageUrl = "https://images.unsplash.com/photo-1520637836862-4d197d17c55a?w=400&h=300&fit=crop",
                    AltText = "Boutique Hotel",
                    IsPrimary = true,
                    DisplayOrder = 1
                },
                new PropertyImage
                {
                    PropertyId = 7,
                    ImageUrl = "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=400&h=300&fit=crop",
                    AltText = "Tree House",
                    IsPrimary = true,
                    DisplayOrder = 1
                },
                new PropertyImage
                {
                    PropertyId = 8,
                    ImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=400&h=300&fit=crop",
                    AltText = "Studio Apartment",
                    IsPrimary = true,
                    DisplayOrder = 1
                }
            };

            context.PropertyImages.AddRange(images);
            context.SaveChanges();
        }
    }
}