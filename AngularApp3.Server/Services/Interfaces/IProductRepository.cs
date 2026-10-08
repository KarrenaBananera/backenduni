using AngularApp3.Server.DTOs.Products;

namespace AngularApp3.Server.Services.Interfaces;

public interface IProductReadRepository
{
    Task<PagedProductsResponse> GetProductsAsync(
        ProductQueryRequest query,
        CancellationToken cancellationToken = default);

}
