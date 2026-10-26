using System.Text.Json;
using AngularApp3.Server.Data;
using AngularApp3.Server.Data.Entities;
using AngularApp3.Server.DTOs.Products;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AngularApp3.Server.Repositories;

public class ProductWriteRepository : IProductWriteRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductWriteRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            Rating = request.Rating,
            Images = request.Images,
            Characteristics = JsonSerializer.Serialize(request.Characteristics),
            StockQuantity = request.StockQuantity,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            Rating = product.Rating,
            Images = product.Images,
            Characteristics = JsonSerializer.Deserialize<Dictionary<string, string>>(product.Characteristics) ?? new(),
            StockQuantity = product.StockQuantity,
            CreatedAt = product.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products
            .Include(p => p.CartItems)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        _dbContext.CartItems.RemoveRange(product.CartItems);
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
