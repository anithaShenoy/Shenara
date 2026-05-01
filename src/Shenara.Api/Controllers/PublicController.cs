using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shenara.Api.Dtos;
using Shenara.Api.Services;

namespace Shenara.Api.Controllers;

[ApiController]
[Route("api")]
public class PublicController(PublicContentService contentService) : ControllerBase
{
    [HttpGet("services")]
    public async Task<ActionResult<IReadOnlyList<ServiceOfferingDto>>> GetServices()
    {
        return Ok(await contentService.GetServicesAsync());
    }

    [HttpGet("gallery")]
    public async Task<ActionResult<IReadOnlyList<GalleryImageDto>>> GetGallery()
    {
        return Ok(await contentService.GetGalleryAsync());
    }

    [HttpPost("inquiries")]
    [EnableRateLimiting("public-inquiries")]
    public async Task<IActionResult> CreateInquiry(CreateInquiryDto dto)
    {
        var result = await contentService.CreateInquiryAsync(dto, HttpContext.Connection.RemoteIpAddress?.ToString());

        if (result.IsDuplicate)
        {
            return Conflict(new { message = result.Message });
        }

        if (result.IsRateLimited)
        {
            return StatusCode(StatusCodes.Status429TooManyRequests, new { message = result.Message });
        }

        return Created($"/api/inquiries/{result.InquiryId}", new { id = result.InquiryId });
    }
}
