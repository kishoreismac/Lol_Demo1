namespace LandOLakesDairyDemo.Models;

public class SeedProductRecord
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string PackageSize { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsFeatured { get; set; }
    public string? ImageFileName { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
}