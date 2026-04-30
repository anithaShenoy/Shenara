using Microsoft.EntityFrameworkCore;
using Shenara.Api.Data;
using Shenara.Api.Dtos;
using Shenara.Api.Models;

namespace Shenara.Api.Services;

public class InventoryService(ShenaraDbContext dbContext)
{
    private const int MaxPageSize = 24;

    public async Task<IReadOnlyList<InventoryCategoryDto>> GetCategoriesAsync()
    {
        return await dbContext.InventoryCategories
            .OrderBy(category => category.Name)
            .Select(category => new InventoryCategoryDto(category.Id, category.Name, category.Description))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetColorsAsync()
    {
        return await dbContext.InventoryItems
            .Where(item => item.Color != null && item.Color != string.Empty)
            .Select(item => item.Color!)
            .Distinct()
            .OrderBy(color => color)
            .ToListAsync();
    }

    public async Task<InventoryPagedResultDto> GetItemsAsync(InventoryQueryDto queryDto)
    {
        var query = dbContext.InventoryItems.Include(item => item.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            query = query.Where(item =>
                item.Name.Contains(queryDto.Search) ||
                (item.Description != null && item.Description.Contains(queryDto.Search)) ||
                (item.Color != null && item.Color.Contains(queryDto.Search)));
        }

        if (queryDto.CategoryId.HasValue)
        {
            query = query.Where(item => item.InventoryCategoryId == queryDto.CategoryId);
        }

        if (queryDto.Status.HasValue)
        {
            query = query.Where(item => item.Status == queryDto.Status);
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Color))
        {
            query = query.Where(item => item.Color == queryDto.Color);
        }

        var page = Math.Max(1, queryDto.Page);
        var pageSize = Math.Clamp(queryDto.PageSize, 1, MaxPageSize);
        var totalCount = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        var normalizedPage = Math.Min(page, totalPages);
        var lowStockCount = await query.CountAsync(item => item.AvailableQuantity <= item.MinimumStockAlert);
        var featuredCount = await query.CountAsync(item => item.IsFeatured);

        var items = await query
            .OrderBy(item => item.Name)
            .Skip((normalizedPage - 1) * pageSize)
            .Take(pageSize)
            .Select(item => ToDto(item))
            .ToListAsync();

        return new InventoryPagedResultDto(items, normalizedPage, pageSize, totalCount, totalPages, lowStockCount, featuredCount);
    }

    public async Task<InventoryItemDto?> GetItemAsync(int id)
    {
        var item = await dbContext.InventoryItems.Include(inventory => inventory.Category).FirstOrDefaultAsync(inventory => inventory.Id == id);
        return item is null ? null : ToDto(item);
    }

    public async Task<InventoryItemDto> CreateItemAsync(UpsertInventoryItemDto dto)
    {
        var item = new InventoryItem
        {
            Name = dto.Name,
            Description = dto.Description,
            InventoryCategoryId = dto.InventoryCategoryId,
            TotalQuantity = dto.TotalQuantity,
            AvailableQuantity = dto.AvailableQuantity,
            MinimumStockAlert = dto.MinimumStockAlert,
            Status = dto.Status,
            PrimaryImageUrl = dto.PrimaryImageUrl,
            Color = dto.Color,
            Size = dto.Size,
            StorageLocation = dto.StorageLocation,
            RentalPrice = dto.RentalPrice,
            IsFeatured = dto.IsFeatured
        };

        dbContext.InventoryItems.Add(item);
        await dbContext.SaveChangesAsync();

        return (await GetItemAsync(item.Id))!;
    }

    public async Task<InventoryItemDto?> UpdateItemAsync(int id, UpsertInventoryItemDto dto)
    {
        var item = await dbContext.InventoryItems.FindAsync(id);

        if (item is null)
        {
            return null;
        }

        item.Name = dto.Name;
        item.Description = dto.Description;
        item.InventoryCategoryId = dto.InventoryCategoryId;
        item.TotalQuantity = dto.TotalQuantity;
        item.AvailableQuantity = dto.AvailableQuantity;
        item.MinimumStockAlert = dto.MinimumStockAlert;
        item.Status = dto.Status;
        item.PrimaryImageUrl = dto.PrimaryImageUrl;
        item.Color = dto.Color;
        item.Size = dto.Size;
        item.StorageLocation = dto.StorageLocation;
        item.RentalPrice = dto.RentalPrice;
        item.IsFeatured = dto.IsFeatured;
        item.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync();

        return await GetItemAsync(id);
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await dbContext.InventoryItems.FindAsync(id);

        if (item is null)
        {
            return false;
        }

        dbContext.InventoryItems.Remove(item);
        await dbContext.SaveChangesAsync();
        return true;
    }

    private static InventoryItemDto ToDto(InventoryItem item)
    {
        return new InventoryItemDto(
            item.Id,
            item.Name,
            item.Description,
            item.InventoryCategoryId,
            item.Category?.Name ?? "Uncategorized",
            item.TotalQuantity,
            item.AvailableQuantity,
            item.MinimumStockAlert,
            item.Status,
            item.PrimaryImageUrl,
            item.Color,
            item.Size,
            item.StorageLocation,
            item.RentalPrice,
            item.IsFeatured,
            item.UpdatedAt);
    }
}
