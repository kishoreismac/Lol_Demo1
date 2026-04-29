using System.ComponentModel.DataAnnotations;

namespace LandOLakesDairyDemo.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string Brand { get; set; } = "Land O Lakes";

    [Required]
    [StringLength(250)]
    public string ShortDescription { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string PackageSize { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "100.00")]
    public decimal Price { get; set; }

    public bool IsFeatured { get; set; }

    [StringLength(150)]
    public string? ImageFileName { get; set; }

    [StringLength(250)]
    public string? Tags { get; set; }

    public DateTime CreatedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }
}