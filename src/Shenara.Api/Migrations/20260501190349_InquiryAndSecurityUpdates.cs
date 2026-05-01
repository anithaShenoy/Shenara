using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shenara.Api.Migrations
{
    /// <inheritdoc />
    public partial class InquiryAndSecurityUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingInquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    EventDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RemoteAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingInquiries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GalleryImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOfferings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(900)", maxLength: 900, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StartingPrice = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOfferings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    InventoryCategoryId = table.Column<int>(type: "int", nullable: false),
                    TotalQuantity = table.Column<int>(type: "int", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "int", nullable: false),
                    MinimumStockAlert = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PrimaryImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    RentalPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_InventoryCategories_InventoryCategoryId",
                        column: x => x.InventoryCategoryId,
                        principalTable: "InventoryCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryItemId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryImages_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GalleryImages",
                columns: new[] { "Id", "EventType", "ImageUrl", "IsFeatured", "Title" },
                values: new object[,]
                {
                    { 1, "Birthday", "https://images.unsplash.com/photo-1530103862676-de8c9debad1d?auto=format&fit=crop&w=900&q=80", true, "Blush birthday backdrop" },
                    { 2, "Reception", "https://images.unsplash.com/photo-1519167758481-83f550bb49b3?auto=format&fit=crop&w=900&q=80", true, "Elegant reception detail" },
                    { 3, "Shower", "https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?auto=format&fit=crop&w=900&q=80", true, "Soft fabric celebration setup" }
                });

            migrationBuilder.InsertData(
                table: "InventoryCategories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Balloon garlands, clusters, and color palettes.", "Balloons" },
                    { 2, "Arch frames and feature backdrops.", "Back Arches" },
                    { 3, "Soft fabric backdrops for layered event scenes.", "Drop Cloths" },
                    { 4, "Accent pieces, stands, and themed decor.", "Props" }
                });

            migrationBuilder.InsertData(
                table: "ServiceOfferings",
                columns: new[] { "Id", "Description", "ImageUrl", "IsFeatured", "Name", "StartingPrice" },
                values: new object[,]
                {
                    { 1, "Custom garlands, clusters, and installations designed around your event colors and venue.", "https://images.unsplash.com/photo-1530103862676-de8c9debad1d?auto=format&fit=crop&w=1200&q=80", true, "Balloon Decor", "From $180" },
                    { 2, "Statement arches with layered balloons, fabric, florals, signage, and themed details.", "https://images.unsplash.com/photo-1527529482837-4698179dc6ce?auto=format&fit=crop&w=1200&q=80", true, "Back Arch Styling", "From $240" },
                    { 3, "Full decor concepts for showers, birthdays, engagements, corporate events, and intimate celebrations.", "https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?auto=format&fit=crop&w=1200&q=80", true, "Custom Event Setups", "Custom quote" }
                });

            migrationBuilder.InsertData(
                table: "InventoryItems",
                columns: new[] { "Id", "AvailableQuantity", "Color", "CreatedAt", "Description", "InventoryCategoryId", "IsFeatured", "MinimumStockAlert", "Name", "PrimaryImageUrl", "RentalPrice", "Size", "Status", "StorageLocation", "TotalQuantity", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 10, "Blush", new DateTimeOffset(new DateTime(2026, 5, 1, 19, 3, 48, 506, DateTimeKind.Unspecified).AddTicks(7681), new TimeSpan(0, 0, 0, 0, 0)), "Soft blush, pearl, and champagne balloon mix for elegant celebrations.", 1, true, 3, "Blush Balloon Garland Kit", "https://images.unsplash.com/photo-1530103862676-de8c9debad1d?auto=format&fit=crop&w=900&q=80", 85m, "Large", 0, "Shelf A1", 12, new DateTimeOffset(new DateTime(2026, 5, 1, 19, 3, 48, 506, DateTimeKind.Unspecified).AddTicks(7684), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2, 2, "Gold", new DateTimeOffset(new DateTime(2026, 5, 1, 19, 3, 48, 506, DateTimeKind.Unspecified).AddTicks(9167), new TimeSpan(0, 0, 0, 0, 0)), "Modern round gold arch for birthdays, showers, engagements, and photo moments.", 2, true, 1, "Round Gold Back Arch", "https://images.unsplash.com/photo-1527529482837-4698179dc6ce?auto=format&fit=crop&w=900&q=80", 120m, "7 ft", 0, "Rack B", 3, new DateTimeOffset(new DateTime(2026, 5, 1, 19, 3, 48, 506, DateTimeKind.Unspecified).AddTicks(9168), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3, 8, "Ivory", new DateTimeOffset(new DateTime(2026, 5, 1, 19, 3, 48, 506, DateTimeKind.Unspecified).AddTicks(9172), new TimeSpan(0, 0, 0, 0, 0)), "Layered ivory backdrop fabric for clean, premium event styling.", 3, false, 2, "Ivory Drop Cloth Set", "https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?auto=format&fit=crop&w=900&q=80", 65m, "10 x 12 ft", 0, "Bin C2", 8, new DateTimeOffset(new DateTime(2026, 5, 1, 19, 3, 48, 506, DateTimeKind.Unspecified).AddTicks(9173), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingInquiries_CreatedAt",
                table: "BookingInquiries",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookingInquiries_Email",
                table: "BookingInquiries",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_BookingInquiries_EventDate",
                table: "BookingInquiries",
                column: "EventDate");

            migrationBuilder.CreateIndex(
                name: "IX_BookingInquiries_RemoteAddress",
                table: "BookingInquiries",
                column: "RemoteAddress");

            migrationBuilder.CreateIndex(
                name: "IX_BookingInquiries_Status",
                table: "BookingInquiries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryImages_InventoryItemId",
                table: "InventoryImages",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_InventoryCategoryId",
                table: "InventoryItems",
                column: "InventoryCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_IsFeatured",
                table: "InventoryItems",
                column: "IsFeatured");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_Status",
                table: "InventoryItems",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingInquiries");

            migrationBuilder.DropTable(
                name: "GalleryImages");

            migrationBuilder.DropTable(
                name: "InventoryImages");

            migrationBuilder.DropTable(
                name: "ServiceOfferings");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "InventoryCategories");
        }
    }
}
