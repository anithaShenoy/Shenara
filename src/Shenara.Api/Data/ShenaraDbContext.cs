using Microsoft.EntityFrameworkCore;
using Shenara.Api.Models;

namespace Shenara.Api.Data;

public class ShenaraDbContext(DbContextOptions<ShenaraDbContext> options) : DbContext(options)
{
    public DbSet<InventoryCategory> InventoryCategories => Set<InventoryCategory>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryImage> InventoryImages => Set<InventoryImage>();
    public DbSet<ServiceOffering> ServiceOfferings => Set<ServiceOffering>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<BookingInquiry> BookingInquiries => Set<BookingInquiry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryItem>().HasIndex(item => item.InventoryCategoryId);
        modelBuilder.Entity<InventoryItem>().HasIndex(item => item.Status);
        modelBuilder.Entity<InventoryItem>().HasIndex(item => item.IsFeatured);
        modelBuilder.Entity<InventoryItem>().Property(item => item.RentalPrice).HasPrecision(10, 2);
        modelBuilder.Entity<BookingInquiry>().HasIndex(inquiry => inquiry.Status);
        modelBuilder.Entity<BookingInquiry>().HasIndex(inquiry => inquiry.EventDate);

        modelBuilder.Entity<InventoryCategory>().HasData(
            new InventoryCategory { Id = 1, Name = "Balloons", Description = "Balloon garlands, clusters, and color palettes." },
            new InventoryCategory { Id = 2, Name = "Back Arches", Description = "Arch frames and feature backdrops." },
            new InventoryCategory { Id = 3, Name = "Drop Cloths", Description = "Soft fabric backdrops for layered event scenes." },
            new InventoryCategory { Id = 4, Name = "Props", Description = "Accent pieces, stands, and themed decor." }
        );

        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem
            {
                Id = 1,
                Name = "Blush Balloon Garland Kit",
                Description = "Soft blush, pearl, and champagne balloon mix for elegant celebrations.",
                InventoryCategoryId = 1,
                TotalQuantity = 12,
                AvailableQuantity = 10,
                MinimumStockAlert = 3,
                PrimaryImageUrl = "https://images.unsplash.com/photo-1530103862676-de8c9debad1d?auto=format&fit=crop&w=900&q=80",
                Color = "Blush",
                Size = "Large",
                StorageLocation = "Shelf A1",
                RentalPrice = 85,
                IsFeatured = true
            },
            new InventoryItem
            {
                Id = 2,
                Name = "Round Gold Back Arch",
                Description = "Modern round gold arch for birthdays, showers, engagements, and photo moments.",
                InventoryCategoryId = 2,
                TotalQuantity = 3,
                AvailableQuantity = 2,
                MinimumStockAlert = 1,
                PrimaryImageUrl = "https://images.unsplash.com/photo-1527529482837-4698179dc6ce?auto=format&fit=crop&w=900&q=80",
                Color = "Gold",
                Size = "7 ft",
                StorageLocation = "Rack B",
                RentalPrice = 120,
                IsFeatured = true
            },
            new InventoryItem
            {
                Id = 3,
                Name = "Ivory Drop Cloth Set",
                Description = "Layered ivory backdrop fabric for clean, premium event styling.",
                InventoryCategoryId = 3,
                TotalQuantity = 8,
                AvailableQuantity = 8,
                MinimumStockAlert = 2,
                PrimaryImageUrl = "https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?auto=format&fit=crop&w=900&q=80",
                Color = "Ivory",
                Size = "10 x 12 ft",
                StorageLocation = "Bin C2",
                RentalPrice = 65
            }
        );

        modelBuilder.Entity<ServiceOffering>().HasData(
            new ServiceOffering
            {
                Id = 1,
                Name = "Balloon Decor",
                Description = "Custom garlands, clusters, and installations designed around your event colors and venue.",
                ImageUrl = "https://images.unsplash.com/photo-1530103862676-de8c9debad1d?auto=format&fit=crop&w=1200&q=80",
                StartingPrice = "From $180",
                IsFeatured = true
            },
            new ServiceOffering
            {
                Id = 2,
                Name = "Back Arch Styling",
                Description = "Statement arches with layered balloons, fabric, florals, signage, and themed details.",
                ImageUrl = "https://images.unsplash.com/photo-1527529482837-4698179dc6ce?auto=format&fit=crop&w=1200&q=80",
                StartingPrice = "From $240",
                IsFeatured = true
            },
            new ServiceOffering
            {
                Id = 3,
                Name = "Custom Event Setups",
                Description = "Full decor concepts for showers, birthdays, engagements, corporate events, and intimate celebrations.",
                ImageUrl = "https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?auto=format&fit=crop&w=1200&q=80",
                StartingPrice = "Custom quote",
                IsFeatured = true
            }
        );

        modelBuilder.Entity<GalleryImage>().HasData(
            new GalleryImage { Id = 1, Title = "Blush birthday backdrop", EventType = "Birthday", ImageUrl = "https://images.unsplash.com/photo-1530103862676-de8c9debad1d?auto=format&fit=crop&w=900&q=80", IsFeatured = true },
            new GalleryImage { Id = 2, Title = "Elegant reception detail", EventType = "Reception", ImageUrl = "https://images.unsplash.com/photo-1519167758481-83f550bb49b3?auto=format&fit=crop&w=900&q=80", IsFeatured = true },
            new GalleryImage { Id = 3, Title = "Soft fabric celebration setup", EventType = "Shower", ImageUrl = "https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?auto=format&fit=crop&w=900&q=80", IsFeatured = true }
        );
    }
}
