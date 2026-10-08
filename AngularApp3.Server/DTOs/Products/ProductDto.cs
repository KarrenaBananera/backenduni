namespace AngularApp3.Server.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public float Rating { get; set; }
    public List<string> Images { get; set; } = [];
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
}
