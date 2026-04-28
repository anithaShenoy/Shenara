using Microsoft.EntityFrameworkCore;
using Shenara.Api.Data;
using Shenara.Api.Dtos;
using Shenara.Api.Models;

namespace Shenara.Api.Services;

public class InventoryService(ShenaraDbContext dbContext)
{
    public async Task<IReadOnlyList<InventoryCategoryDto>> GetCategoriesAsync()
    {
        return await dbContext.InventoryCategories
            .OrderBy(category => category.Name)
            .Select(category => new InventoryCategoryDto(category.Id, category.Name, category.Description))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InventoryItemDto>> GetItemsAsync(string? search, int? categoryId, InventoryStatus? status)
    {
        var query = dbContext.InventoryItems.Include(item => item.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item => item.Name.Contains(search) || (item.Description != null && item.Description.Contains(search)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(item => item.InventoryCategoryId == categoryId);
        }

        if (status.HasValue)
        {
            query = query.Where(item => item.Status == status);
        }

        return await query
            .OrderBy(item => item.Name)
            .Select(item => ToDto(item))
            .ToListAsync();
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
