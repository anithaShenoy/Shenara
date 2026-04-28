using System.ComponentModel.DataAnnotations;

namespace Shenara.Api.Models;

public class InventoryCategory
{
    public int Id { get; set; }

    [MaxLength(80)]
    public required string Name { get; set; }

    [MaxLength(220)]
    public string? Description { get; set; }

    public ICollection<InventoryItem> Items { get; set; } = new List<InventoryItem>();
}
