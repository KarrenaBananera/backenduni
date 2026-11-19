using System.ComponentModel.DataAnnotations;

namespace AngularApp3.Server.DTOs.PromoCodes;

public class CreatePromoCodeRequest : IValidatableObject
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    public decimal DiscountPercentage { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DiscountPercentage <= 0 || DiscountPercentage >= 100)
        {
            yield return new ValidationResult(
                "Discount percentage must be greater than 0 and less than 100.",
                [nameof(DiscountPercentage)]);
        }
    }
}
