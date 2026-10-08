using AngularApp3.Server.DTOs.Products;

namespace AngularApp3.Server.Services.Interfaces;

public interface IProductService
{
    Task<PagedProductsResponse> GetProductsAsync(
        ProductQueryRequest query,
        CancellationToken cancellationToken = default);
}
