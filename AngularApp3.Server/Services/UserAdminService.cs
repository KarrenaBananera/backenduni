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

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(user =>
                (user.UserName != null && EF.Functions.ILike(user.UserName, pattern)) ||
                (user.Email != null && EF.Functions.ILike(user.Email, pattern)));
        }

        var users = await query
            .OrderBy(user => user.UserName)
            .ToListAsync(cancellationToken);

        var result = new List<UserDto>(users.Count);
        foreach (var user in users)
        {
            result.Add(await ToDtoAsync(user));
        }

        return result;
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
