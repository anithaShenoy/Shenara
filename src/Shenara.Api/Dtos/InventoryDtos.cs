using System.ComponentModel.DataAnnotations;
using Shenara.Api.Models;

namespace Shenara.Api.Dtos;

public record InventoryCategoryDto(int Id, string Name, string? Description);

public record InventoryItemDto(
    int Id,
    string Name,
    string? Description,
    int InventoryCategoryId,
    string CategoryName,
    int TotalQuantity,
    int AvailableQuantity,
    int MinimumStockAlert,
    InventoryStatus Status,
    string? PrimaryImageUrl,
    string? Color,
    string? Size,
    string? StorageLocation,
    decimal? RentalPrice,
    bool IsFeatured,
    DateTimeOffset UpdatedAt);

public class UpsertInventoryItemDto
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(800)]
    public string? Description { get; set; }

    [Range(1, int.MaxValue)]
    public int InventoryCategoryId { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalQuantity { get; set; }

    [Range(0, int.MaxValue)]
    public int AvailableQuantity { get; set; }

    [Range(0, int.MaxValue)]
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
}
