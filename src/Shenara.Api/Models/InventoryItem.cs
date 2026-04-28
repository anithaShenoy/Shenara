using System.ComponentModel.DataAnnotations;

namespace Shenara.Api.Models;

public class InventoryItem
{
    public int Id { get; set; }

    [MaxLength(120)]
    public required string Name { get; set; }

    [MaxLength(800)]
    public string? Description { get; set; }

    public int InventoryCategoryId { get; set; }
    public InventoryCategory? Category { get; set; }
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int MinimumStockAlert { get; set; }
    public InventoryStatus Status { get; set; } = InventoryStatus.Active;

    [MaxLength(500)]
    public string? PrimaryImageUrl { get; set; }

    [MaxLength(80)]
    public string? Color { get; set; }

    [MaxLength(80)]
    public string? Size { get; set; }

    [MaxLength(120)]
    public string? StorageLocation { get; set; }

    public decimal? RentalPrice { get; set; }
    public bool IsFeatured { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<InventoryImage> Images { get; set; } = new List<InventoryImage>();
}
