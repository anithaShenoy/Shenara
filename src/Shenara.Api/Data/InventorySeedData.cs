using Microsoft.EntityFrameworkCore;
using Shenara.Api.Models;
using System.Text.RegularExpressions;

namespace Shenara.Api.Data;

public static class InventorySeedData
{
    private const string InvoiceStorageLocation = "Invoice CAN88167";

    private static readonly string[] InvoiceItems =
    [
        "9\" Decomex Capybara Stuffed Plush Animal (Dexie)",
        "12\" Kalisan Latex Balloons Standard Chocolate Brown (50 Per Bag)",
        "5\" Kalisan Latex Balloons Standard Chocolate Brown (50 Per Bag)",
        "260D Deco Peach Decomex Twisting Modelling Latex Balloons (100 Per Bag)",
        "260K Kalisan Twisting Latex Balloons Pastel Matte Macaron Yellow (50 Per Bag)",
        "12\" Metallic Silver Decomex Latex Balloons (100 Per Bag)",
        "9\" Metallic Silver Decomex Latex Balloons (100 Per Bag)",
        "5\" Metallic Silver Decomex Latex Balloons (100 Per Bag)",
        "14\" Airfill Only Megaloon Jr. Shape Number 1 Gold Balloon",
        "18\" Heart Mother's Day Spring Floral Foil Balloon",
        "18\" Mother's Day Pink & Gold Dots Foil Balloon",
        "18\" Heart Mother's Day Pink Floral Foil Balloon",
        "6\" Clear Linking Latex Balloons by Decomex (100 Count)",
        "100-Pack of 5\" Crystal Clear Decomex Latex Balloons",
        "260D Deco Matte Mint Green Decomex Twisting Modelling Latex Balloons (100 Per Bag)",
        "260D Deco Cameo Decomex Twisting Modelling Latex Balloons (100 Per Bag)",
        "260D Deco Baby Pink Decomex Twisting Modelling Latex Balloons (100 Per Bag)",
        "11\" Standard Red Decomex Heart Shaped Latex Balloons (100 Per Bag)",
        "12\" Standard Red Decomex Latex Balloons (100 Per Bag)",
        "9\" Standard Red Decomex Latex Balloons (100 Per Bag)",
        "5\" Standard Red Decomex Latex Balloons (100 Per Bag)",
        "9\" Deco Light Pink Decomex Latex Balloons (100 Per Bag)",
        "12\" Deco Sage Decomex Latex Balloons (100 Per Bag)",
        "9\" Deco Sage Decomex Latex Balloons (100 Per Bag)",
        "5\" Deco Sage Decomex Latex Balloons (100 Per Bag)",
        "6\" Metallic Pearl White Decomex Linking Latex Balloons (100 Per Bag)",
        "12\" Deco Light Blue Decomex Latex Balloons (100 Per Bag)",
        "9\" Deco Light Blue Decomex Latex Balloons (100 Per Bag)",
        "12\" Metallic Gold Decomex Latex Balloons (100 Per Bag)",
        "9\" Metallic Gold Decomex Latex Balloons (100 Per Bag)",
        "5\" Metallic Gold Decomex Latex Balloons (100 Per Bag)",
        "12\" Deco Floral Decomex Latex Balloons (100 Per Bag)",
        "9\" Deco Floral Decomex Latex Balloons (100 Per Bag)",
        "5\" Deco Floral Decomex Latex Balloons (100 Per Bag)",
        "5\" Kalisan Latex Balloons Standard Black (50 Per Bag)",
        "12\" Standard Black Decomex Latex Balloons (100 Per Bag)",
        "9\" Standard Black Decomex Latex Balloons (100 Per Bag)",
        "5\" Standard Black Decomex Latex Balloons (100 Per Bag)",
        "9\" Metallic Pearl White Decomex Latex Balloons (100 Per Bag)",
        "5\" Metallic Pearl White Decomex Latex Balloons (100 Per Bag)",
        "12\" Deco Sand Decomex Latex Balloons (100 Per Bag)",
        "9\" Deco Sand Decomex Latex Balloons (100 Per Bag)",
        "5\" Deco Light Pink Decomex Latex Balloons (100 Per Bag)",
        "12\" Deco Dusty Rose Decomex Latex Balloons (100 Per Bag)",
        "5\" Deco Dusty Rose Decomex Latex Balloons (100 Per Bag)",
        "5\" Deco Light Blue Decomex Latex Balloons (100 Per Bag)",
        "260D Deco Dusty White Decomex Twisting Modelling Latex Balloons (100 Per Bag)",
        "9\" Standard White Decomex Latex Balloons (100 Per Bag)",
        "5\" Standard White Decomex Latex Balloons (100 Per Bag)",
        "18\" Happy Birthday Gold Stars Balloons",
        "17\" Happy Birthday Geometric Foil Balloon",
        "6\" Standard White Decomex Linking Latex Balloons (100 Per Bag)",
        "5\" Kalisan Latex Balloons Standard Pastel Assortment (50 Per Bag)",
        "34\" Northstar Brand Packaged Number 0 - Rose Gold Balloon",
        "34\" Northstar Brand Packaged Number 2 - Rose Gold Balloon",
        "9\" Deco Dusty Rose Decomex Latex Balloons (100 Per Bag)",
        "17\" Welcome Little Boy Foil Balloon"
    ];

    public static async Task SeedInvoiceInventoryAsync(ShenaraDbContext dbContext)
    {
        var categories = await dbContext.InventoryCategories.ToDictionaryAsync(category => category.Name);
        var balloonCategoryId = categories["Balloons"].Id;
        var propsCategoryId = categories["Props"].Id;
        var invoiceItemNames = InvoiceItems.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingNames = await dbContext.InventoryItems
            .Select(item => item.Name)
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase);

        foreach (var itemName in InvoiceItems.Where(itemName => !existingNames.Contains(itemName)))
        {
            var quantity = InferQuantity(itemName);

            dbContext.InventoryItems.Add(new InventoryItem
            {
                Name = itemName,
                Description = $"Imported from {InvoiceStorageLocation}.",
                InventoryCategoryId = itemName.Contains("Stuffed Plush", StringComparison.OrdinalIgnoreCase) ? propsCategoryId : balloonCategoryId,
                TotalQuantity = quantity,
                AvailableQuantity = quantity,
                MinimumStockAlert = quantity >= 50 ? 10 : 1,
                Status = InventoryStatus.Active,
                Color = InferColor(itemName),
                PrimaryImageUrl = BuildInventoryImageUrl(itemName),
                Size = InferSize(itemName),
                StorageLocation = InvoiceStorageLocation
            });
        }

        var existingInvoiceItems = await dbContext.InventoryItems
            .Where(item => item.StorageLocation == InvoiceStorageLocation)
            .ToListAsync();

        foreach (var item in existingInvoiceItems.Where(item => invoiceItemNames.Contains(item.Name) && string.IsNullOrWhiteSpace(item.PrimaryImageUrl)))
        {
            item.PrimaryImageUrl = BuildInventoryImageUrl(item.Name);
            item.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await dbContext.SaveChangesAsync();
    }

    private static int InferQuantity(string itemName)
    {
        var packageMatch = Regex.Match(itemName, @"\((\d+)\s*(Per Bag|Count)\)", RegexOptions.IgnoreCase);

        if (packageMatch.Success && int.TryParse(packageMatch.Groups[1].Value, out var packageQuantity))
        {
            return packageQuantity;
        }

        var packMatch = Regex.Match(itemName, @"^(\d+)-Pack", RegexOptions.IgnoreCase);

        if (packMatch.Success && int.TryParse(packMatch.Groups[1].Value, out var packQuantity))
        {
            return packQuantity;
        }

        return 1;
    }

    private static string? InferSize(string itemName)
    {
        var sizeMatch = Regex.Match(itemName, @"^(\d+""|260D|260K)", RegexOptions.IgnoreCase);
        return sizeMatch.Success ? sizeMatch.Groups[1].Value : null;
    }

    private static string? InferColor(string itemName)
    {
        var colors = new[]
        {
            "Chocolate Brown",
            "Pastel Matte Macaron Yellow",
            "Metallic Silver",
            "Spring Floral",
            "Pink & Gold Dots",
            "Pink Floral",
            "Crystal Clear",
            "Clear",
            "Matte Mint Green",
            "Cameo",
            "Baby Pink",
            "Standard Red",
            "Red",
            "Light Pink",
            "Sage",
            "Metallic Pearl White",
            "Light Blue",
            "Metallic Gold",
            "Floral",
            "Standard Black",
            "Black",
            "Sand",
            "Dusty Rose",
            "Dusty White",
            "Standard White",
            "White",
            "Gold",
            "Pastel Assortment",
            "Rose Gold",
            "Little Boy"
        };

        return colors.FirstOrDefault(color => itemName.Contains(color, StringComparison.OrdinalIgnoreCase));
    }

    private static string BuildInventoryImageUrl(string itemName)
    {
        var (background, foreground) = InferImageColors(itemName);
        var label = BuildImageLabel(itemName);

        return $"https://placehold.co/600x600/{background}/{foreground}.png?text={Uri.EscapeDataString(label)}";
    }

    private static string BuildImageLabel(string itemName)
    {
        var size = InferSize(itemName);
        var color = InferColor(itemName);
        var productType = itemName.Contains("Stuffed Plush", StringComparison.OrdinalIgnoreCase)
            ? "Plush"
            : itemName.Contains("Foil", StringComparison.OrdinalIgnoreCase)
                ? "Foil"
                : itemName.Contains("Twisting", StringComparison.OrdinalIgnoreCase)
                    ? "Twisting"
                    : "Balloons";

        return string.Join(" ", new[] { size, color, productType }.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static (string Background, string Foreground) InferImageColors(string itemName)
    {
        var color = InferColor(itemName) ?? string.Empty;

        if (itemName.Contains("Stuffed Plush", StringComparison.OrdinalIgnoreCase))
        {
            return ("9B6A53", "FFF8EF");
        }

        return color switch
        {
            "Chocolate Brown" => ("5A3825", "FFF8EF"),
            "Pastel Matte Macaron Yellow" => ("F8DD6C", "3B2F1C"),
            "Metallic Silver" => ("C9CDD2", "28201D"),
            "Spring Floral" => ("F3C3D5", "5E3040"),
            "Pink & Gold Dots" => ("F4AFC4", "6E4B00"),
            "Pink Floral" => ("F5A8BE", "5E3040"),
            "Crystal Clear" => ("DFF7FF", "295867"),
            "Clear" => ("DFF7FF", "295867"),
            "Matte Mint Green" => ("A8DCC4", "1F4A39"),
            "Cameo" => ("D7B9A9", "4B3229"),
            "Baby Pink" => ("F8B8D0", "6B2F45"),
            "Standard Red" => ("C82432", "FFF8EF"),
            "Red" => ("C82432", "FFF8EF"),
            "Light Pink" => ("F7BCD0", "6B2F45"),
            "Sage" => ("A7B89A", "263821"),
            "Metallic Pearl White" => ("F6F1E8", "5D554B"),
            "Light Blue" => ("9DD6F4", "1D4664"),
            "Metallic Gold" => ("D6A33F", "3F2A00"),
            "Floral" => ("F0B7C8", "4B5334"),
            "Standard Black" => ("171717", "FFFFFF"),
            "Black" => ("171717", "FFFFFF"),
            "Sand" => ("D8BE9A", "4A3829"),
            "Dusty Rose" => ("C98691", "FFF8EF"),
            "Dusty White" => ("EEE7DB", "5D554B"),
            "Standard White" => ("F8F6EF", "5D554B"),
            "White" => ("F8F6EF", "5D554B"),
            "Gold" => ("D6A33F", "3F2A00"),
            "Pastel Assortment" => ("BFD9F2", "58446F"),
            "Rose Gold" => ("C98A7D", "FFF8EF"),
            "Little Boy" => ("8CC7E8", "1D4664"),
            _ => ("EADFD5", "28201D")
        };
    }
}
