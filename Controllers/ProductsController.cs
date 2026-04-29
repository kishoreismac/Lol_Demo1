using LandOLakesDairyDemo.Models;
using LandOLakesDairyDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace LandOLakesDairyDemo.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? category)
    {
        var viewModel = new ProductCatalogViewModel
        {
            SearchTerm = searchTerm,
            SelectedCategory = category,
            Categories = await _productService.GetCategoriesAsync(),
            Products = await _productService.GetProductsAsync(searchTerm, category)
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }
}