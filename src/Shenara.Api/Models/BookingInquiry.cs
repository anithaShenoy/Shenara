using System.ComponentModel.DataAnnotations;

namespace Shenara.Api.Models;

public class BookingInquiry
{
    public int Id { get; set; }

    [MaxLength(120)]
    public required string CustomerName { get; set; }

    [MaxLength(160)]
    public required string Email { get; set; }

    [MaxLength(40)]
    public string? Phone { get; set; }

    public DateOnly? EventDate { get; set; }

    [MaxLength(100)]
    public string? EventType { get; set; }

    [MaxLength(1000)]
    public required string Message { get; set; }

    [MaxLength(64)]
    public string? RemoteAddress { get; set; }

    public InquiryStatus Status { get; set; } = InquiryStatus.New;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
