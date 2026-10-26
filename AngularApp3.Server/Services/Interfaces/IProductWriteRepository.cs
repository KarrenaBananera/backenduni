using AngularApp3.Server.DTOs.Products;

namespace AngularApp3.Server.Services.Interfaces;

public interface IProductWriteRepository
{
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
