using AngularApp3.Server.DTOs.Products;
using AngularApp3.Server.Identity;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp3.Server.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ICachedProductService _productService;
    private readonly IProductWriteRepository _productWriteRepository;

    public ProductsController(
        ICachedProductService productService,
        IProductWriteRepository productWriteRepository)
    {
        _productService = productService;
        _productWriteRepository = productWriteRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedProductsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedProductsResponse>> GetProducts(
        [FromQuery] ProductQueryRequest query,
        CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductsAsync(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        if (request.Images.Any(image => !IsHttpUrl(image)))
        {
            return BadRequest(new { error = "Images must contain at least 3 http(s) URLs." });
        }

        if (request.Characteristics.Any(item => string.IsNullOrWhiteSpace(item.Key) || string.IsNullOrWhiteSpace(item.Value)))
        {
            return BadRequest(new { error = "Characteristic keys and values must not be empty." });
        }

        var product = await _productWriteRepository.CreateAsync(request, cancellationToken);
        await _productService.InvalidateCatalogAsync(cancellationToken);
        return Created($"/api/products/{product.Id}", product);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _productWriteRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        await _productService.InvalidateCatalogAsync(cancellationToken);
        return NoContent();
    }

    private static bool IsHttpUrl(string value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
