using AngularApp3.Server.Common;
using AngularApp3.Server.Data;
using AngularApp3.Server.Data.Entities;
using AngularApp3.Server.DTOs.Cart;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AngularApp3.Server.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _dbContext;

    public CartService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceResult<CartDto>> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        return ServiceResult<CartDto>.Success(await MapAsync(cart.Id, cancellationToken));
    }

    public async Task<ServiceResult<CartDto>> AddItemAsync(
        string userId,
        AddCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null)
        {
            return ServiceResult<CartDto>.Failure("Product not found.", StatusCodes.Status404NotFound);
        }

        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        var quantity = (item?.Quantity ?? 0) + request.Quantity;

        if (quantity > product.StockQuantity)
        {
            return ServiceResult<CartDto>.Failure("Insufficient stock.", StatusCodes.Status400BadRequest);
        }

        if (item is null)
        {
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.Quantity
            });
        }
        else
        {
            item.Quantity = quantity;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<CartDto>.Success(await MapAsync(cart.Id, cancellationToken));
    }

    public async Task<ServiceResult<CartDto>> UpdateItemAsync(
        string userId,
        Guid itemId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        var item = cart.Items.FirstOrDefault(i => i.Id == itemId);

        if (item is null)
        {
            return ServiceResult<CartDto>.Failure("Cart item not found.", StatusCodes.Status404NotFound);
        }

        if (request.Quantity > item.Product.StockQuantity)
        {
            return ServiceResult<CartDto>.Failure("Insufficient stock.", StatusCodes.Status400BadRequest);
        }

        item.Quantity = request.Quantity;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<CartDto>.Success(await MapAsync(cart.Id, cancellationToken));
    }

    public async Task<ServiceResult<CartDto>> RemoveItemAsync(
        string userId,
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        var item = cart.Items.FirstOrDefault(i => i.Id == itemId);

        if (item is null)
        {
            return ServiceResult<CartDto>.Failure("Cart item not found.", StatusCodes.Status404NotFound);
        }

        _dbContext.CartItems.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<CartDto>.Success(await MapAsync(cart.Id, cancellationToken));
    }

    public async Task<ServiceResult<CartDto>> ApplyPromoCodeAsync(
        string userId,
        ApplyPromoCodeRequest request,
        CancellationToken cancellationToken = default)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        var promo = await _dbContext.PromoCodes
            .FirstOrDefaultAsync(p => p.Code.ToUpper() == code && p.IsActive, cancellationToken);

        if (promo is null)
        {
            return ServiceResult<CartDto>.Failure("Promo code is invalid.", StatusCodes.Status400BadRequest);
        }

        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        cart.AppliedPromoCode = promo.Code;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<CartDto>.Success(await MapAsync(cart.Id, cancellationToken));
    }

    private async Task<Cart> GetOrCreateCartAsync(string userId, CancellationToken cancellationToken)
    {
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is not null)
        {
            return cart;
        }

        cart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Carts.Add(cart);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return cart;
    }

    private async Task<CartDto> MapAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var cart = await _dbContext.Carts.AsNoTracking()
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstAsync(c => c.Id == cartId, cancellationToken);

        var items = cart.Items
            .Select(item => new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Title = item.Product.Title,
                Price = item.Product.Price,
                Quantity = item.Quantity,
                LineTotal = item.Product.Price * item.Quantity
            })
            .ToList();

        var subtotal = items.Sum(item => item.LineTotal);
        decimal discountPercentage = 0;

        if (!string.IsNullOrWhiteSpace(cart.AppliedPromoCode))
        {
            var promo = await _dbContext.PromoCodes.AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.Code == cart.AppliedPromoCode && p.IsActive,
                    cancellationToken);

            if (promo is not null)
            {
                discountPercentage = promo.DiscountPercentage;
            }
        }

        var discountAmount = Math.Round(subtotal * discountPercentage / 100m, 2);

        return new CartDto
        {
            Id = cart.Id,
            Items = items,
            Subtotal = subtotal,
            PromoCode = cart.AppliedPromoCode,
            DiscountPercentage = discountPercentage,
            DiscountAmount = discountAmount,
            Total = subtotal - discountAmount
        };
    }
}
