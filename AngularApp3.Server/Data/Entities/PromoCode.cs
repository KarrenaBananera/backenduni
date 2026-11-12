using Microsoft.EntityFrameworkCore;

namespace AngularApp3.Server.Data.Entities;

[Index(nameof(Code), IsUnique = true)]
public class PromoCode
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Code { get; set; } = string.Empty;

    public decimal DiscountPercentage { get; set; }

    public bool IsActive { get; set; } = true;
}
