using LandOLakesDairyDemo.Models;
using LandOLakesDairyDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace LandOLakesDairyDemo.Controllers;

public class AdminProductsController : Controller
{
    private readonly IProductService _productService;

    public AdminProductsController(IProductService productService)
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

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _productService.GetCategoriesAsync();
        return View(new ProductInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductInputModel inputModel)
    {
        ViewBag.Categories = await _productService.GetCategoriesAsync();

        if (!ModelState.IsValid)
        {
            return View(inputModel);
        }

        var result = await _productService.CreateAsync(inputModel);
        AddServiceErrors(result.Errors);

        if (!result.Success)
        {
            return View(inputModel);
        }

        TempData["StatusMessage"] = $"{result.Product!.ProductName} was added to the catalog.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var inputModel = await _productService.GetInputModelAsync(id);
        if (inputModel is null)
        {
            return NotFound();
        }

        ViewBag.Categories = await _productService.GetCategoriesAsync();
        ViewBag.ProductRecordId = id;
        return View(inputModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductInputModel inputModel)
    {
        ViewBag.Categories = await _productService.GetCategoriesAsync();
        ViewBag.ProductRecordId = id;

        if (!ModelState.IsValid)
        {
            return View(inputModel);
        }

        var result = await _productService.UpdateAsync(id, inputModel);
        AddServiceErrors(result.Errors);

        if (!result.Success)
        {
            if (result.Errors.ContainsKey("id"))
            {
                return NotFound();
            }

            return View(inputModel);
        }

        TempData["StatusMessage"] = $"{result.Product!.ProductName} was updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "The product was removed from the catalog.";
        return RedirectToAction(nameof(Index));
    }

    private void AddServiceErrors(Dictionary<string, string[]> errors)
    {
        foreach (var error in errors)
        {
            foreach (var message in error.Value)
            {
                ModelState.AddModelError(error.Key, message);
            }
        }
    }
}