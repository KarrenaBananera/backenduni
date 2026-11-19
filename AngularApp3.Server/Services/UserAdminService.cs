using AngularApp3.Server.Common;
using AngularApp3.Server.Data.Entities;
using AngularApp3.Server.DTOs.Users;
using AngularApp3.Server.Identity;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AngularApp3.Server.Services;

public class UserAdminService : IUserAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserAdminService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<PagedUsersResponse> GetUsersAsync(UserQueryRequest request, CancellationToken cancellationToken = default)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var limit = request.Limit < 1 ? 10 : request.Limit;
        var query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var pattern = $"%{request.Search.Trim()}%";
            query = query.Where(user =>
                (user.UserName != null && EF.Functions.ILike(user.UserName, pattern)) ||
                (user.Email != null && EF.Functions.ILike(user.Email, pattern)));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)limit);
        var users = await query
            .OrderBy(user => user.UserName)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var items = new List<UserDto>(users.Count);
        foreach (var user in users)
        {
            items.Add(await ToDtoAsync(user));
        }

        return new PagedUsersResponse
        {
            Items = items,
            Meta = new DTOs.Products.PaginationMeta
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                Limit = limit
            }
        };
    }

    public async Task<ServiceResult<UserDto>> GrantAdminAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ServiceResult<UserDto>.Failure("User not found.", StatusCodes.Status404NotFound);
        }

        if (!await _userManager.IsInRoleAsync(user, AppRoles.Admin))
        {
            var addResult = await _userManager.AddToRoleAsync(user, AppRoles.Admin);
            if (!addResult.Succeeded)
            {
                var error = string.Join(" ", addResult.Errors.Select(e => e.Description));
                return ServiceResult<UserDto>.Failure(error, StatusCodes.Status400BadRequest);
            }
        }

        return ServiceResult<UserDto>.Success(await ToDtoAsync(user));
    }

    private async Task<UserDto> ToDtoAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList(),
            CreatedAt = user.CreatedAt
        };
    }
}
