namespace LandOLakesDairyDemo.Models;

public class ProductCatalogViewModel
{
    public string? SearchTerm { get; set; }

    public string? SelectedCategory { get; set; }

    public IReadOnlyList<string> Categories { get; set; } = Array.Empty<string>();

    public IReadOnlyList<Product> Products { get; set; } = Array.Empty<Product>();
}