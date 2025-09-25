using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using StayFinder.Data;
using StayFinder.Interfaces;
using StayFinder.Repositories;
using StayFinder.Services;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Entity Framework with warning suppression
builder.Services.AddDbContext<StayFinderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("customer"));
    options.AddPolicy("HostOnly", policy => policy.RequireRole("host"));
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("superadmin"));
    options.AddPolicy("HostOrAdmin", policy => policy.RequireRole("host", "superadmin"));
    options.AddPolicy("CustomerOrHost", policy => policy.RequireRole("customer", "host"));
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IOfferRepository, OfferRepository>();
builder.Services.AddScoped<IAmenityRepository, AmenityRepository>();
builder.Services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IPropertyAmenityRepository, PropertyAmenityRepository>();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger with JWT Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "StayFinder API", 
        Version = "v1",
        Description = "A comprehensive property booking API for StayFinder platform"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StayFinder API V1");
        c.RoutePrefix = "swagger";
    });
}

// Comment out HTTPS redirection to avoid port issues during development
// app.UseHttpsRedirection();

// Add static files middleware
app.UseStaticFiles();

// Enable CORS
app.UseCors("AllowAll");

// Add Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Default route to serve homepage
app.MapGet("/", () => Results.Redirect("/home.html"));

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StayFinderDbContext>();
    try
    {
        context.Database.EnsureCreated();
        
        // Seed database if empty
        await SeedDatabase(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database setup error: {ex.Message}");
    }
}

// Auto-open browser if configured
var appSettings = builder.Configuration.GetSection("ApplicationSettings");
if (appSettings.GetValue<bool>("AutoOpenBrowser"))
{
    var defaultUrl = appSettings["DefaultUrl"] ?? "/home.html";
    var baseUrl = app.Environment.IsDevelopment() ? "https://localhost:7076" : "https://localhost:5001";
    var fullUrl = $"{baseUrl}{defaultUrl}";
    
    try
    {
        Process.Start(new ProcessStartInfo(fullUrl) { UseShellExecute = true });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not open browser: {ex.Message}");
    }
}

app.Run();

// Database seeding method
async Task SeedDatabase(StayFinderDbContext context)
{
    // Check if database is already seeded
    if (context.Users.Any())
        return;

    // Seed SuperAdmin
    var superAdmin = new StayFinder.Models.User
    {
        FirstName = "Super",
        LastName = "Admin",
        Email = "admin@stayfinder.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
        Role = "superadmin",
        IsActive = true,
        IsHostApproved = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    context.Users.Add(superAdmin);

    // Seed Sample Customer
    var customer = new StayFinder.Models.User
    {
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        Phone = "+1234567890",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
        Role = "customer",
        IsActive = true,
        IsHostApproved = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    context.Users.Add(customer);

    // Seed Sample Host
    var host = new StayFinder.Models.User
    {
        FirstName = "Jane",
        LastName = "Smith",
        Email = "jane@example.com",
        Phone = "+1234567891",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
        Role = "host",
        IsActive = true,
        IsHostApproved = true,
        Address = "123 Main St, New York, NY",
        Bio = "Experienced host with 5+ years of hospitality experience",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    context.Users.Add(host);

    await context.SaveChangesAsync();

    // Seed Amenities
    var amenities = new[]
    {
        new StayFinder.Models.Amenity { Name = "WiFi", Category = "Technology", Icon = "wifi" },
        new StayFinder.Models.Amenity { Name = "Air Conditioning", Category = "Climate", Icon = "ac" },
        new StayFinder.Models.Amenity { Name = "Swimming Pool", Category = "Recreation", Icon = "pool" },
        new StayFinder.Models.Amenity { Name = "Gym", Category = "Recreation", Icon = "gym" },
        new StayFinder.Models.Amenity { Name = "Parking", Category = "Transportation", Icon = "parking" },
        new StayFinder.Models.Amenity { Name = "Kitchen", Category = "Kitchen", Icon = "kitchen" },
        new StayFinder.Models.Amenity { Name = "Washer/Dryer", Category = "Laundry", Icon = "laundry" },
        new StayFinder.Models.Amenity { Name = "Pet Friendly", Category = "Policies", Icon = "pet" }
    };

    context.Amenities.AddRange(amenities);
    await context.SaveChangesAsync();

    Console.WriteLine("Database seeded successfully!");
}
