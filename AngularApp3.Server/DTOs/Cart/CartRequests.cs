using System.ComponentModel.DataAnnotations;

namespace AngularApp3.Server.DTOs.Cart;

public class AddCartItemRequest
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemRequest
{
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

public class ApplyPromoCodeRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
}
