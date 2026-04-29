using System.ComponentModel.DataAnnotations;

namespace LandOLakesDairyDemo.Models;

public class ProductInputModel
{
    [Required]
    [RegularExpression(@"^LLD\d{3}$", ErrorMessage = "Product ID must use the format LLD001.")]
    [Display(Name = "Product ID")]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Category")]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string Brand { get; set; } = "Land O Lakes";

    [Required]
    [StringLength(250)]
    [Display(Name = "Short Description")]
    public string ShortDescription { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    [Display(Name = "Package Size")]
    public string PackageSize { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "100.00")]
    public decimal Price { get; set; }

    [Display(Name = "Featured Product")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Image File Name")]
    [StringLength(150)]
    public string? ImageFileName { get; set; }

    [Display(Name = "Tags")]
    [StringLength(250)]
    public string? Tags { get; set; }
}