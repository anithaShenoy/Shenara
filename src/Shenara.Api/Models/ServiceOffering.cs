using System.ComponentModel.DataAnnotations;

namespace Shenara.Api.Models;

public class ServiceOffering
{
    public int Id { get; set; }

    [MaxLength(120)]
    public required string Name { get; set; }

    [MaxLength(900)]
    public required string Description { get; set; }

    [MaxLength(500)]
    public required string ImageUrl { get; set; }

    [MaxLength(80)]
    public string? StartingPrice { get; set; }

    public bool IsFeatured { get; set; }
}
