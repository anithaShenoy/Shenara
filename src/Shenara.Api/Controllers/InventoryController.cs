using Microsoft.AspNetCore.Mvc;
using Shenara.Api.Dtos;
using Shenara.Api.Filters;
using Shenara.Api.Models;
using Shenara.Api.Services;

namespace Shenara.Api.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController(InventoryService inventoryService) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<InventoryCategoryDto>>> GetCategories()
    {
        return Ok(await inventoryService.GetCategoriesAsync());
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InventoryItemDto>>> GetItems([FromQuery] string? search, [FromQuery] int? categoryId, [FromQuery] InventoryStatus? status)
    {
        return Ok(await inventoryService.GetItemsAsync(search, categoryId, status));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InventoryItemDto>> GetItem(int id)
    {
        var item = await inventoryService.GetItemAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [ServiceFilter(typeof(AdminTokenFilter))]
    public async Task<ActionResult<InventoryItemDto>> CreateItem(UpsertInventoryItemDto dto)
    {
        var item = await inventoryService.CreateItemAsync(dto);
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(AdminTokenFilter))]
    public async Task<ActionResult<InventoryItemDto>> UpdateItem(int id, UpsertInventoryItemDto dto)
    {
        var item = await inventoryService.UpdateItemAsync(id, dto);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpDelete("{id:int}")]
    [ServiceFilter(typeof(AdminTokenFilter))]
    public async Task<IActionResult> DeleteItem(int id)
    {
        return await inventoryService.DeleteItemAsync(id) ? NoContent() : NotFound();
    }
}
