using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LandOLakesDairyDemo.Models;
using LandOLakesDairyDemo.Services;

namespace LandOLakesDairyDemo.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;

    public HomeController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel
        {
            FeaturedProducts = await _productService.GetFeaturedProductsAsync(6),
            Categories = await _productService.GetCategoriesAsync()
        };

        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
