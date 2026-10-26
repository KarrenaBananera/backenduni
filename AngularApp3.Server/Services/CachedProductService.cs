using System.Text.Json;
using AngularApp3.Server.Configuration;
using AngularApp3.Server.DTOs.Products;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AngularApp3.Server.Services;

public class CachedProductService : ICachedProductService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IProductReadRepository _products;
    private readonly IConnectionMultiplexer _redis;
    private readonly RedisOptions _options;

    public CachedProductService(
        IProductReadRepository products,
        IConnectionMultiplexer redis,
        IOptions<RedisOptions> options)
    {
        _products = products;
        _redis = redis;
        _options = options.Value;
    }

    public async Task<PagedProductsResponse> GetProductsAsync(
        ProductQueryRequest query,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = ProductCacheKey.ForQuery(_options.KeyPrefix, query);
        var database = _redis.GetDatabase();

        var cached = await TryGetAsync(database, cacheKey);
        if (cached is not null)
        {
            return cached;
        }

        var result = await _products.GetProductsAsync(query, cancellationToken);
        await TrySetAsync(database, cacheKey, result);
        return result;
    }

    public async Task InvalidateCatalogAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var database = _redis.GetDatabase();
            var indexKey = ProductCacheKey.Index(_options.KeyPrefix);
            var keys = await database.SetMembersAsync(indexKey);
            var redisKeys = keys
                .Select(key => (RedisKey)(string)key!)
                .Append((RedisKey)indexKey)
                .ToArray();

            await database.KeyDeleteAsync(redisKeys);
        }
        catch (RedisException)
        {
        }
    }

    private async Task<PagedProductsResponse?> TryGetAsync(IDatabase database, string cacheKey)
    {
        try
        {
            var cached = await database.StringGetAsync(cacheKey);
            if (!cached.HasValue)
            {
                return null;
            }

            return JsonSerializer.Deserialize<PagedProductsResponse>((string)cached!, JsonOptions);
        }
        catch (Exception exception) when (exception is RedisException or JsonException)
        {
            return null;
        }
    }

    private async Task TrySetAsync(IDatabase database, string cacheKey, PagedProductsResponse result)
    {
        try
        {
            var payload = JsonSerializer.Serialize(result, JsonOptions);
            var ttl = TimeSpan.FromSeconds(Math.Max(1, _options.CatalogTtlSeconds));

            await database.StringSetAsync(cacheKey, payload, ttl);
            await database.SetAddAsync(ProductCacheKey.Index(_options.KeyPrefix), cacheKey);
        }
        catch (RedisException)
        {
        }
    }
}
