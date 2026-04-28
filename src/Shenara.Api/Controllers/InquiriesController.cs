using Microsoft.AspNetCore.Mvc;
using Shenara.Api.Dtos;
using Shenara.Api.Filters;
using Shenara.Api.Models;
using Shenara.Api.Services;

namespace Shenara.Api.Controllers;

[ApiController]
[Route("api/inquiries")]
[ServiceFilter(typeof(AdminTokenFilter))]
public class InquiriesController(PublicContentService contentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookingInquiryDto>>> GetInquiries([FromQuery] InquiryStatus? status)
    {
        return Ok(await contentService.GetInquiriesAsync(status));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<BookingInquiryDto>> UpdateStatus(int id, UpdateInquiryStatusDto dto)
    {
        var inquiry = await contentService.UpdateInquiryStatusAsync(id, dto.Status);
        return inquiry is null ? NotFound() : Ok(inquiry);
    }
}
