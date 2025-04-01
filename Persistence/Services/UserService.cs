using Application.Core;
using Application.DTOs.Users;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;

    public UserService(
        UserManager<AppUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
    }

    public async Task<Result<string>> ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<string>.Failure("User not found", 404);

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<string>.Failure(string.Join("; ", errors), 400);
        }

        await RevokeCurrentTokenAsync();
        return Result<string>.Success("Password changed successfully");
    }

    public async Task<Result<string>> UpdateProfileAsync(string userId, UpdateUserProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<string>.Failure("User not found", 404);

        bool emailChanged = false;

        if (!string.IsNullOrWhiteSpace(dto.NewEmail) && dto.NewEmail != user.Email)
        {
            user.Email = dto.NewEmail;
            user.UserName = dto.NewEmail;
            emailChanged = true;
        }

        if (!string.IsNullOrWhiteSpace(dto.NewPhoneNumber))
        {
            user.PhoneNumber = dto.NewPhoneNumber;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<string>.Failure(string.Join("; ", errors), 400);
        }

        if (emailChanged)
        {
            await RevokeCurrentTokenAsync();
        }

        return Result<string>.Success("Profile updated successfully");
    }

    public async Task<Result<string>> ToggleTwoFactorAsync(string userId, Toggle2FaDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<string>.Failure("User not found", 404);

        var result = await _userManager.SetTwoFactorEnabledAsync(user, dto.IsEnabled);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<string>.Failure(string.Join("; ", errors), 400);
        }

        var status = dto.IsEnabled ? "enabled" : "disabled";
        return Result<string>.Success($"Two-factor authentication {status} successfully");
    }

    private async Task RevokeCurrentTokenAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        if (!string.IsNullOrWhiteSpace(token))
        {
            await _tokenService.RevokeTokenAsync(token);
        }
    }
}
