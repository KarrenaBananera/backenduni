using AngularApp3.Server.DTOs.Products;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp3.Server.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
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
}
