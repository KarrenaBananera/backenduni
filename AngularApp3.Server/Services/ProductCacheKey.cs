using System.Globalization;
using AngularApp3.Server.DTOs.Products;

namespace AngularApp3.Server.Services;

public static class ProductCacheKey
{
    public const string IndexSuffix = "index";

    public static string ForQuery(string keyPrefix, ProductQueryRequest query)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var limit = query.Limit < 1 ? 12 : query.Limit;

        var parameters = new (string Name, string? Value)[]
        {
            ("page", page.ToString(CultureInfo.InvariantCulture)),
            ("limit", limit.ToString(CultureInfo.InvariantCulture)),
            ("search", Normalize(query.Search)),
            ("min_price", query.MinPrice?.ToString(CultureInfo.InvariantCulture)),
            ("max_price", query.MaxPrice?.ToString(CultureInfo.InvariantCulture)),
            ("min_rating", query.MinRating?.ToString(CultureInfo.InvariantCulture)),
            ("sort_by", Normalize(query.SortBy)),
            ("order", Normalize(query.Order))
        };

        var queryString = string.Join(
            '&',
            parameters
                .Where(parameter => !string.IsNullOrEmpty(parameter.Value))
                .Select(parameter => $"{parameter.Name}={Uri.EscapeDataString(parameter.Value!)}"));

        return $"{keyPrefix}{queryString}";
    }

    public static string Index(string keyPrefix) => $"{keyPrefix}{IndexSuffix}";

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
}
