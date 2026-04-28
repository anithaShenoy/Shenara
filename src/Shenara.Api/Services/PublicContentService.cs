using Microsoft.EntityFrameworkCore;
using Shenara.Api.Data;
using Shenara.Api.Dtos;
using Shenara.Api.Models;

namespace Shenara.Api.Services;

public class PublicContentService(ShenaraDbContext dbContext)
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

    public async Task<int> CreateInquiryAsync(CreateInquiryDto dto)
    {
        var inquiry = new BookingInquiry
        {
            CustomerName = dto.CustomerName,
            Email = dto.Email,
            Phone = dto.Phone,
            EventDate = dto.EventDate,
            EventType = dto.EventType,
            Message = dto.Message
        };

        dbContext.BookingInquiries.Add(inquiry);
        await dbContext.SaveChangesAsync();

        return inquiry.Id;
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
}
