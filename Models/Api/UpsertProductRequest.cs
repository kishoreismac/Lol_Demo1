using System.ComponentModel.DataAnnotations;

namespace LandOLakesDairyDemo.Models.Api;

public class UpsertProductRequest
{
    [Required]
    [RegularExpression(@"^LLD\d{3}$")]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public string Brand { get; set; } = string.Empty;

    [Required]
    public string ShortDescription { get; set; } = string.Empty;

    [Required]
    public string PackageSize { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "100.00")]
    public decimal Price { get; set; }

    public bool IsFeatured { get; set; }

    public string? ImageFileName { get; set; }

    public string[] Tags { get; set; } = Array.Empty<string>();
}