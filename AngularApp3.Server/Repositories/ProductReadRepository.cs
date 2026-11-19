using System.Text.Json;
using AngularApp3.Server.Data;
using AngularApp3.Server.Data.Entities;
using AngularApp3.Server.DTOs.Products;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AngularApp3.Server.Repositories;

public class ProductReadRepository : IProductReadRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductReadRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedProductsResponse> GetProductsAsync(
        ProductQueryRequest query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var limit = query.Limit < 1 ? 12 : query.Limit;

        var products = _dbContext.Products.AsNoTracking().AsQueryable();

        products = ApplyFilters(products, query);
        products = ApplySorting(products, query.SortBy, query.Order);

        var totalItems = await products.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)limit);

        var pageItems = await products
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var items = pageItems.Select(ToDto).ToList();

        return new PagedProductsResponse
        {
            Items = items,
            Meta = new PaginationMeta
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                Limit = limit
            }
        };
    }

    private static IQueryable<Product> ApplyFilters(IQueryable<Product> query, ProductQueryRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var pattern = $"%{request.Search.Trim()}%";
            query = query.Where(p =>
                EF.Functions.ILike(p.Title, pattern) ||
                EF.Functions.ILike(p.Description, pattern));
        }

        if (request.MinPrice is not null)
        {
            query = query.Where(p => p.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice is not null)
        {
            query = query.Where(p => p.Price <= request.MaxPrice.Value);
        }

        if (request.MinRating is not null)
        {
            query = query.Where(p => p.Rating >= request.MinRating.Value);
        }

        return query;
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return product is null ? null : ToDto(product);
    }

    private static ProductDto ToDto(Product product) => new()
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

    private static IQueryable<Product> ApplySorting(
        IQueryable<Product> query,
        string? sortBy,
        string? order)
    {
        var descending = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);

        return (sortBy?.Trim().ToLowerInvariant()) switch
        {
            "title" => descending
                ? query.OrderByDescending(p => p.Title)
                : query.OrderBy(p => p.Title),
            "price" => descending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };
    }
}
