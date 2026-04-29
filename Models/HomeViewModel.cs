namespace LandOLakesDairyDemo.Models;

public class HomeViewModel
{
    public IReadOnlyList<Product> FeaturedProducts { get; set; } = Array.Empty<Product>();

    public IReadOnlyList<string> Categories { get; set; } = Array.Empty<string>();
}