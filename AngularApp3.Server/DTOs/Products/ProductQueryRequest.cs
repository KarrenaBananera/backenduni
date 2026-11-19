using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp3.Server.DTOs.Products;

public class ProductQueryRequest
{
    [FromQuery(Name = "page")]
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "limit")]
    [Range(1, 100)]
    public int Limit { get; set; } = 12;

    [FromQuery(Name = "search")]
    public string? Search { get; set; }

    [FromQuery(Name = "min_price")]
    [Range(0, double.MaxValue)]
    public decimal? MinPrice { get; set; }

    [FromQuery(Name = "max_price")]
    [Range(0, double.MaxValue)]
    public decimal? MaxPrice { get; set; }

    [FromQuery(Name = "min_rating")]
    [Range(0, 5)]
    public float? MinRating { get; set; }

    [FromQuery(Name = "sort_by")]
    public string? SortBy { get; set; }

    [FromQuery(Name = "order")]
    public string? Order { get; set; }
}
