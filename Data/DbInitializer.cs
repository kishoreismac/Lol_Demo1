using System.Text.Json;
using LandOLakesDairyDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace LandOLakesDairyDemo.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext dbContext, string contentRootPath)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Products.AnyAsync())
        {
            return;
        }

        var seedFilePath = Path.Combine(contentRootPath, "seed-catalog.json");
        if (!File.Exists(seedFilePath))
        {
            return;
        }

        var seedJson = await File.ReadAllTextAsync(seedFilePath);
        var seedRecords = JsonSerializer.Deserialize<List<SeedProductRecord>>(seedJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<SeedProductRecord>();

        var timestamp = DateTime.UtcNow;

        var products = seedRecords.Select(record => new Product
        {
            ProductId = record.ProductId,
            ProductName = record.ProductName,
            Category = record.Category,
            Brand = record.Brand,
            ShortDescription = record.ShortDescription,
            PackageSize = record.PackageSize,
            Price = record.Price,
            IsFeatured = record.IsFeatured,
            ImageFileName = record.ImageFileName,
            Tags = string.Join(", ", record.Tags),
            CreatedUtc = timestamp,
            UpdatedUtc = timestamp
        });

        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();
    }
}