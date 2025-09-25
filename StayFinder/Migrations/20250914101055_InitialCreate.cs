using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StayFinder.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "properties",
                columns: table => new
                {
                    property_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    rating = table.Column<decimal>(type: "decimal(2,1)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    available_from = table.Column<DateTime>(type: "datetime2", nullable: false),
                    available_to = table.Column<DateTime>(type: "datetime2", nullable: false),
                    image_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_properties", x => x.property_id);
                    table.CheckConstraint("CK_Properties_Type", "[type] IN ('apartment', 'villa', 'room')");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "customer"),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.CheckConstraint("CK_Users_Role", "[role] IN ('customer', 'host', 'admin')");
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    booking_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    property_id = table.Column<int>(type: "int", nullable: false),
                    check_in = table.Column<DateTime>(type: "datetime2", nullable: false),
                    check_out = table.Column<DateTime>(type: "datetime2", nullable: false),
                    guests = table.Column<int>(type: "int", nullable: false),
                    total_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Confirmed"),
                    booking_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bookings", x => x.booking_id);
                    table.CheckConstraint("CK_Bookings_Status", "[status] IN ('Confirmed', 'Pending', 'Cancelled', 'Completed')");
                    table.ForeignKey(
                        name: "FK_Bookings_Properties",
                        column: x => x.property_id,
                        principalTable: "properties",
                        principalColumn: "property_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "properties",
                columns: new[] { "property_id", "available_from", "available_to", "city", "description", "image_path", "location", "price", "type", "rating", "title" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "New York", "Beautiful modern apartment with city views, fully furnished with modern amenities. Perfect for business travelers.", "files/images/1.jpg", "123 Main Street", 150.00m, "apartment", 4.5m, "Modern Apartment in Downtown" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Miami", "Stunning villa with private pool and ocean view. Spacious living areas and luxury amenities.", "files/images/2.jpg", "456 Ocean Drive", 300.00m, "villa", 4.8m, "Luxury Villa with Pool" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "San Francisco", "Comfortable private room in shared house with friendly hosts. Great location near public transport.", "files/images/3.jpg", "789 Park Avenue", 80.00m, "room", 4.2m, "Cozy Private Room" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chicago", "Luxury penthouse with panoramic city views. Premium location in downtown area.", "files/images/4.jpg", "101 Skyline Boulevard", 450.00m, "apartment", 4.9m, "Penthouse Suite" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Los Angeles", "Beautiful beach house with direct beach access. Perfect for family vacations.", "files/images/5.jpg", "202 Coastal Road", 250.00m, "villa", 4.6m, "Beach House Getaway" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "user_id", "created_at", "email", "first_name", "is_active", "last_name", "password_hash", "phone", "role", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 14, 10, 10, 53, 92, DateTimeKind.Utc).AddTicks(7815), "ahmadzubayer007@gmail.com", "Ahmad", true, "Zubayer", "hashed_12345", "123-456-7890", "customer", new DateTime(2025, 9, 14, 10, 10, 53, 92, DateTimeKind.Utc).AddTicks(7909) },
                    { 2, new DateTime(2025, 9, 14, 10, 10, 53, 92, DateTimeKind.Utc).AddTicks(7990), "jane.smith@example.com", "Jane", true, "Smith", "hashed_password", "987-654-3210", "host", new DateTime(2025, 9, 14, 10, 10, 53, 92, DateTimeKind.Utc).AddTicks(7990) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingReference",
                table: "bookings",
                column: "booking_reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bookings_property_id",
                table: "bookings",
                column: "property_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_user_id",
                table: "bookings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "properties");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
