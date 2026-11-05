using AngularApp3.Server.Common;
using AngularApp3.Server.DTOs.Users;

namespace AngularApp3.Server.Services.Interfaces;

public interface IUserAdminService
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync(string? search, CancellationToken cancellationToken = default);
    Task<ServiceResult<UserDto>> GrantAdminAsync(string userId, CancellationToken cancellationToken = default);
}
