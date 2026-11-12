using System.Text.Json.Serialization;
using AngularApp3.Server.DTOs.Products;

namespace AngularApp3.Server.DTOs.Users;

public class PagedUsersResponse
{
    [JsonPropertyName("items")]
    public IReadOnlyList<UserDto> Items { get; set; } = [];

    [JsonPropertyName("meta")]
    public PaginationMeta Meta { get; set; } = new();
}
