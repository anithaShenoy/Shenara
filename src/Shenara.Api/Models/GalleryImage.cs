using System.ComponentModel.DataAnnotations;

namespace Shenara.Api.Models;

public class GalleryImage
{
    public int Id { get; set; }

    [MaxLength(120)]
    public required string Title { get; set; }

    [MaxLength(500)]
    public required string ImageUrl { get; set; }

    [MaxLength(80)]
    public string? EventType { get; set; }

    public bool IsFeatured { get; set; }
}
