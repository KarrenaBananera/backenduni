using AngularApp3.Server.Common;
using AngularApp3.Server.Data;
using AngularApp3.Server.Data.Entities;
using AngularApp3.Server.DTOs.PromoCodes;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AngularApp3.Server.Services;

public class PromoCodeService : IPromoCodeService
{
    private readonly ApplicationDbContext _dbContext;

    public PromoCodeService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<PromoCodeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.PromoCodes.AsNoTracking()
            .OrderBy(promo => promo.Code)
            .Select(promo => new PromoCodeDto
            {
                Id = promo.Id,
                Code = promo.Code,
                DiscountPercentage = promo.DiscountPercentage,
                IsActive = promo.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceResult<PromoCodeDto>> CreateAsync(
        CreatePromoCodeRequest request,
        CancellationToken cancellationToken = default)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        var exists = await _dbContext.PromoCodes
            .AnyAsync(promo => promo.Code.ToUpper() == code, cancellationToken);

        if (exists)
        {
            return ServiceResult<PromoCodeDto>.Failure("Promo code already exists.", StatusCodes.Status409Conflict);
        }

        var promoCode = new PromoCode
        {
            Code = code,
            DiscountPercentage = request.DiscountPercentage,
            IsActive = true
        };

        _dbContext.PromoCodes.Add(promoCode);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<PromoCodeDto>.Success(new PromoCodeDto
        {
            Id = promoCode.Id,
            Code = promoCode.Code,
            DiscountPercentage = promoCode.DiscountPercentage,
            IsActive = promoCode.IsActive
        }, StatusCodes.Status201Created);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var promoCode = await _dbContext.PromoCodes.FirstOrDefaultAsync(promo => promo.Id == id, cancellationToken);
        if (promoCode is null)
        {
            return false;
        }

        _dbContext.PromoCodes.Remove(promoCode);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
