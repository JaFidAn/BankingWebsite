using Application.Core;
using Application.DTOs.Users;

namespace Application.Services;

public interface IUserService
{
    Task<Result<string>> ChangePasswordAsync(string userId, ChangePasswordDto dto);
    Task<Result<string>> UpdateProfileAsync(string userId, UpdateUserProfileDto dto);
    Task<Result<string>> ToggleTwoFactorAsync(string userId, Toggle2FaDto dto);
}
