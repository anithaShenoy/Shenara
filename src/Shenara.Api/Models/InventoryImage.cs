using System.ComponentModel.DataAnnotations;

namespace Shenara.Api.Models;

public class InventoryImage
{
    public int Id { get; set; }
    public int InventoryItemId { get; set; }
    public InventoryItem? InventoryItem { get; set; }

    [MaxLength(500)]
    public required string Url { get; set; }

    [MaxLength(160)]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }
}
