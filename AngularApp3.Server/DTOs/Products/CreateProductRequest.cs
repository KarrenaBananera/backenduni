using System.ComponentModel.DataAnnotations;

namespace AngularApp3.Server.DTOs.Products;

public class CreateProductRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, 5)]
    public float Rating { get; set; }

    [Required]
    [MinLength(3)]
    public List<string> Images { get; set; } = [];

    public Dictionary<string, string> Characteristics { get; set; } = new();

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
}
