using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AngularApp3.Server.Data.Entities;

[Index(nameof(UserId), IsUnique = true)]
public class Cart
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? AppliedPromoCode { get; set; }

    public ICollection<CartItem> Items { get; set; } = [];
}
