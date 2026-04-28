using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> CreateInquiry(CreateInquiryDto dto)
    {
        var id = await contentService.CreateInquiryAsync(dto);
        return Created($"/api/inquiries/{id}", new { id });
    }
}
