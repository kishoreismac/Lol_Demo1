using LandOLakesDairyDemo.Models;
using LandOLakesDairyDemo.Models.Api;
using LandOLakesDairyDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace LandOLakesDairyDemo.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsApiController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsApiController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] string? search, [FromQuery] string? category, [FromQuery] bool featuredOnly = false)
    {
        var products = await _productService.GetProductsAsync(search, category, featuredOnly);
        return Ok(products.Select(MapToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(product));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(UpsertProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var inputModel = MapToInputModel(request);
        var result = await _productService.CreateAsync(inputModel);

        if (!result.Success)
        {
            return BuildErrorResponse(result.Errors);
        }

        return CreatedAtAction(nameof(GetProduct), new { id = result.Product!.Id }, MapToDto(result.Product));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpsertProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var inputModel = MapToInputModel(request);
        var result = await _productService.UpdateAsync(id, inputModel);

        if (!result.Success)
        {
            if (result.Errors.ContainsKey("id"))
            {
                return NotFound();
            }

            return BuildErrorResponse(result.Errors);
        }

        return Ok(MapToDto(result.Product!));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private ActionResult BuildErrorResponse(Dictionary<string, string[]> errors)
    {
        return BadRequest(new
        {
            message = "Validation failed.",
            errors
        });
    }

    private static ProductInputModel MapToInputModel(UpsertProductRequest request)
    {
        return new ProductInputModel
        {
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            Category = request.Category,
            Brand = request.Brand,
            ShortDescription = request.ShortDescription,
            PackageSize = request.PackageSize,
            Price = request.Price,
            IsFeatured = request.IsFeatured,
            ImageFileName = request.ImageFileName,
            Tags = string.Join(", ", request.Tags ?? Array.Empty<string>())
        };
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Category = product.Category,
            Brand = product.Brand,
            ShortDescription = product.ShortDescription,
            PackageSize = product.PackageSize,
            Price = product.Price,
            IsFeatured = product.IsFeatured,
            ImageFileName = product.ImageFileName,
            Tags = (product.Tags ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        };
    }
}