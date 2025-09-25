using Microsoft.EntityFrameworkCore;
using StayFinder.Models;

namespace StayFinder.Data
{
    public class StayFinderDbContext : DbContext
    {
        public StayFinderDbContext(DbContextOptions<StayFinderDbContext> options) : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<PropertyAmenity> PropertyAmenities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Property configuration
            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("properties", t => 
                    t.HasCheckConstraint("CK_Properties_Type", 
                        "[type] IN ('apartment', 'villa', 'room')"));

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("property_id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasColumnName("description");

                entity.Property(e => e.PropertyType)
                    .HasColumnName("type")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Rating)
                    .HasColumnName("rating")
                    .HasColumnType("decimal(2,1)");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                entity.Property(e => e.AvailableFrom)
                    .HasColumnName("available_from")
                    .IsRequired();

                entity.Property(e => e.AvailableTo)
                    .HasColumnName("available_to")
                    .IsRequired();

                entity.Property(e => e.ImagePath)
                    .HasColumnName("image_path")
                    .HasMaxLength(500);
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("user_id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasMaxLength(50)
                    .HasDefaultValue("customer");

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                // Create unique index on email
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");

                // Add check constraint for role
                entity.ToTable("users", t => 
                    t.HasCheckConstraint("CK_Users_Role", 
                        "[role] IN ('customer', 'host', 'admin')"));
            });

            // Booking configuration
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("bookings");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("booking_id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.BookingReference)
                    .HasColumnName("booking_reference")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(50)
                    .HasDefaultValue("Confirmed");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                // Foreign key relationships
                entity.HasOne(b => b.User)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Bookings_Users");

                entity.HasOne(b => b.Property)
                    .WithMany()
                    .HasForeignKey(b => b.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Bookings_Properties");

                // Create unique index on booking reference
                entity.HasIndex(e => e.BookingReference)
                    .IsUnique()
                    .HasDatabaseName("IX_Bookings_BookingReference");

                // Add check constraint for status
                entity.ToTable("bookings", t => 
                    t.HasCheckConstraint("CK_Bookings_Status", 
                        "[status] IN ('Confirmed', 'Pending', 'Cancelled', 'Completed')"));
            });

            // Review configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("review_id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.PropertyId)
                    .HasColumnName("property_id")
                    .IsRequired();

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.HostId)
                    .HasColumnName("host_id");

                entity.Property(e => e.BookingId)
                    .HasColumnName("booking_id")
                    .IsRequired();

                entity.Property(e => e.Rating)
                    .HasColumnName("rating")
                    .IsRequired();

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasMaxLength(1000);

                entity.Property(e => e.IsApproved)
                    .HasColumnName("is_approved")
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                // Foreign key relationships
                entity.HasOne(r => r.Property)
                    .WithMany()
                    .HasForeignKey(r => r.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reviews_Properties");

                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reviews_Users_Customer");

                entity.HasOne(r => r.Host)
                    .WithMany()
                    .HasForeignKey(r => r.HostId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reviews_Users_Host");

                entity.HasOne(r => r.Booking)
                    .WithMany()
                    .HasForeignKey(r => r.BookingId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reviews_Bookings");

                // Add check constraint for rating
                entity.ToTable("reviews", t => 
                    t.HasCheckConstraint("CK_Reviews_Rating", 
                        "[rating] BETWEEN 1 AND 5"));
            });

            // Seed data for properties (keeping existing seed data)
            modelBuilder.Entity<Property>().HasData(
                new Property
                {
                    Id = 1,
                    Title = "Modern Apartment in Downtown",
                    Location = "123 Main Street",
                    City = "New York",
                    Description = "Beautiful modern apartment with city views, fully furnished with modern amenities. Perfect for business travelers.",
                    PropertyType = "apartment",
                    Rating = 4.5m,
                    Price = 150.00m,
                    AvailableFrom = new DateTime(2025, 1, 1),
                    AvailableTo = new DateTime(2025, 7, 1),
                    ImagePath = "files/images/1.jpg"
                },
                new Property
                {
                    Id = 2,
                    Title = "Luxury Villa with Pool",
                    Location = "456 Ocean Drive",
                    City = "Miami",
                    Description = "Stunning villa with private pool and ocean view. Spacious living areas and luxury amenities.",
                    PropertyType = "villa",
                    Rating = 4.8m,
                    Price = 300.00m,
                    AvailableFrom = new DateTime(2025, 1, 1),
                    AvailableTo = new DateTime(2025, 4, 1),
                    ImagePath = "files/images/2.jpg"
                },
                new Property
                {
                    Id = 3,
                    Title = "Cozy Private Room",
                    Location = "789 Park Avenue",
                    City = "San Francisco",
                    Description = "Comfortable private room in shared house with friendly hosts. Great location near public transport.",
                    PropertyType = "room",
                    Rating = 4.2m,
                    Price = 80.00m,
                    AvailableFrom = new DateTime(2025, 1, 1),
                    AvailableTo = new DateTime(2026, 1, 1),
                    ImagePath = "files/images/3.jpg"
                },
                new Property
                {
                    Id = 4,
                    Title = "Penthouse Suite",
                    Location = "101 Skyline Boulevard",
                    City = "Chicago",
                    Description = "Luxury penthouse with panoramic city views. Premium location in downtown area.",
                    PropertyType = "apartment",
                    Rating = 4.9m,
                    Price = 450.00m,
                    AvailableFrom = new DateTime(2025, 1, 1),
                    AvailableTo = new DateTime(2025, 12, 31),
                    ImagePath = "files/images/4.jpg"
                },
                new Property
                {
                    Id = 5,
                    Title = "Beach House Getaway",
                    Location = "202 Coastal Road",
                    City = "Los Angeles",
                    Description = "Beautiful beach house with direct beach access. Perfect for family vacations.",
                    PropertyType = "villa",
                    Rating = 4.6m,
                    Price = 250.00m,
                    AvailableFrom = new DateTime(2025, 1, 1),
                    AvailableTo = new DateTime(2025, 8, 31),
                    ImagePath = "files/images/5.jpg"
                }
            );

            // Seed data for users (demo users)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Ahmad",
                    LastName = "Zubayer",
                    Email = "ahmadzubayer007@gmail.com",
                    Phone = "123-456-7890",
                    PasswordHash = "hashed_12345", // In real implementation, this should be properly hashed
                    Role = "customer",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    Phone = "987-654-3210",
                    PasswordHash = "hashed_password",
                    Role = "host",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}