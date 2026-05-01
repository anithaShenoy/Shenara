using Microsoft.EntityFrameworkCore;
using Shenara.Api.Data;
using Shenara.Api.Dtos;
using Shenara.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace Shenara.Api.Services;

public class PublicContentService(ShenaraDbContext dbContext, InquiryProtectionService inquiryProtectionService)
{
    public async Task<IReadOnlyList<ServiceOfferingDto>> GetServicesAsync()
    {
        return await dbContext.ServiceOfferings
            .OrderByDescending(service => service.IsFeatured)
            .ThenBy(service => service.Name)
            .Select(service => new ServiceOfferingDto(service.Id, service.Name, service.Description, service.ImageUrl, service.StartingPrice, service.IsFeatured))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<GalleryImageDto>> GetGalleryAsync()
    {
        return await dbContext.GalleryImages
            .OrderByDescending(image => image.IsFeatured)
            .ThenBy(image => image.Title)
            .Select(image => new GalleryImageDto(image.Id, image.Title, image.ImageUrl, image.EventType, image.IsFeatured))
            .ToListAsync();
    }

    public async Task<InquiryCreateResult> CreateInquiryAsync(CreateInquiryDto dto, string? remoteAddress)
    {
        var nowUtc = DateTimeOffset.UtcNow;
        var startOfDayUtc = new DateTimeOffset(nowUtc.UtcDateTime.Date, TimeSpan.Zero);
        var fingerprint = CreateFingerprint(dto);
        var protectionResult = inquiryProtectionService.Evaluate(remoteAddress, fingerprint, nowUtc);

        if (!protectionResult.IsAllowed)
        {
            return InquiryCreateResult.TooManyRequests(protectionResult.Message!);
        }

        var submittedToday = await dbContext.BookingInquiries.AnyAsync(inquiry =>
            inquiry.CreatedAt >= startOfDayUtc &&
            inquiry.Email == dto.Email &&
            inquiry.CustomerName == dto.CustomerName &&
            inquiry.Phone == dto.Phone &&
            inquiry.Message == dto.Message &&
            inquiry.EventDate == dto.EventDate &&
            inquiry.EventType == dto.EventType);

        if (submittedToday)
        {
            return InquiryCreateResult.Duplicate(
                "A matching inquiry was already submitted today. Shenara Event Decor will reach out soon.");
        }

        var inquiry = new BookingInquiry
        {
            CustomerName = dto.CustomerName,
            Email = dto.Email,
            Phone = dto.Phone,
            EventDate = dto.EventDate,
            EventType = dto.EventType,
            Message = dto.Message,
            RemoteAddress = remoteAddress
        };

        dbContext.BookingInquiries.Add(inquiry);
        await dbContext.SaveChangesAsync();

        return InquiryCreateResult.Created(inquiry.Id);
    }

    public async Task<IReadOnlyList<BookingInquiryDto>> GetInquiriesAsync(InquiryStatus? status)
    {
        var query = dbContext.BookingInquiries.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(inquiry => inquiry.Status == status);
        }

        return await query
            .OrderByDescending(inquiry => inquiry.CreatedAt)
            .Select(inquiry => ToDto(inquiry))
            .ToListAsync();
    }

    public async Task<BookingInquiryDto?> UpdateInquiryStatusAsync(int id, InquiryStatus status)
    {
        var inquiry = await dbContext.BookingInquiries.FindAsync(id);

        if (inquiry is null)
        {
            return null;
        }

        inquiry.Status = status;
        await dbContext.SaveChangesAsync();

        return ToDto(inquiry);
    }

    private static BookingInquiryDto ToDto(BookingInquiry inquiry)
    {
        return new BookingInquiryDto(
            inquiry.Id,
            inquiry.CustomerName,
            inquiry.Email,
            inquiry.Phone,
            inquiry.EventDate,
            inquiry.EventType,
            inquiry.Message,
            inquiry.Status,
            inquiry.CreatedAt);
    }

    private static string CreateFingerprint(CreateInquiryDto dto)
    {
        var raw = string.Join("|", new[]
        {
            dto.CustomerName.Trim(),
            dto.Email.Trim().ToLowerInvariant(),
            dto.Phone?.Trim() ?? string.Empty,
            dto.EventDate?.ToString() ?? string.Empty,
            dto.EventType?.Trim() ?? string.Empty,
            dto.Message.Trim()
        });

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
    }
}

public record InquiryCreateResult(bool IsSuccess, bool IsDuplicate, bool IsRateLimited, int? InquiryId, string? Message)
{
    public static InquiryCreateResult Created(int id) => new(true, false, false, id, null);
    public static InquiryCreateResult Duplicate(string message) => new(false, true, false, null, message);
    public static InquiryCreateResult TooManyRequests(string message) => new(false, false, true, null, message);
}
