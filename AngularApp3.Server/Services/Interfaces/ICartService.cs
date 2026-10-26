using AngularApp3.Server.Common;
using AngularApp3.Server.DTOs.Cart;

namespace AngularApp3.Server.Services.Interfaces;

public interface ICartService
{
    Task<ServiceResult<CartDto>> GetAsync(string userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<CartDto>> AddItemAsync(string userId, AddCartItemRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<CartDto>> UpdateItemAsync(string userId, Guid itemId, UpdateCartItemRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<CartDto>> RemoveItemAsync(string userId, Guid itemId, CancellationToken cancellationToken = default);
    Task<ServiceResult<CartDto>> ApplyPromoCodeAsync(string userId, ApplyPromoCodeRequest request, CancellationToken cancellationToken = default);
}
