using System.Text.Json.Serialization;

namespace AngularApp3.Server.DTOs.Products;

public class PagedProductsResponse
{
    [JsonPropertyName("items")]
    public IReadOnlyList<ProductDto> Items { get; set; } = [];

    [JsonPropertyName("meta")]
    public PaginationMeta Meta { get; set; } = new();
}

public class PaginationMeta
{
    [JsonPropertyName("total_items")]
    public int TotalItems { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
