using System.ComponentModel.DataAnnotations;
using Shenara.Api.Models;

namespace Shenara.Api.Dtos;

public record ServiceOfferingDto(int Id, string Name, string Description, string ImageUrl, string? StartingPrice, bool IsFeatured);

public record GalleryImageDto(int Id, string Title, string ImageUrl, string? EventType, bool IsFeatured);

public record BookingInquiryDto(
    int Id,
    string CustomerName,
    string Email,
    string? Phone,
    DateOnly? EventDate,
    string? EventType,
    string Message,
    InquiryStatus Status,
    DateTimeOffset CreatedAt);

public class CreateInquiryDto
{
    [Required, MaxLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(40)]
    public string? Phone { get; set; }

    public DateOnly? EventDate { get; set; }

    [MaxLength(100)]
    public string? EventType { get; set; }

    [Required, MaxLength(1000)]
    public string Message { get; set; } = string.Empty;
}

public class UpdateInquiryStatusDto
{
    public InquiryStatus Status { get; set; }
}
