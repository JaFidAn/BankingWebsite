using Application.DTOs.Users;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _userService.ChangePasswordAsync(userId!, dto);
        return HandleResult(result);
    }

    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile(UpdateUserProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _userService.UpdateProfileAsync(userId!, dto);
        return HandleResult(result);
    }

    [HttpPost("toggle-2fa")]
    public async Task<IActionResult> ToggleTwoFactor(Toggle2FaDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _userService.ToggleTwoFactorAsync(userId!, dto);
        return HandleResult(result);
    }
}
