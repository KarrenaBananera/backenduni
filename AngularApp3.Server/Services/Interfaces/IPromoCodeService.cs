using AngularApp3.Server.Common;
using AngularApp3.Server.DTOs.PromoCodes;

namespace AngularApp3.Server.Services.Interfaces;

public interface IPromoCodeService
{
    Task<IReadOnlyList<PromoCodeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<PromoCodeDto>> CreateAsync(CreatePromoCodeRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
