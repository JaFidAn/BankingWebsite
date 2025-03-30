using Application.Core;
using Application.DTOs.Accounts;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

public class AccountsController : BaseApiController
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _accountService.CreateAsync(userId!, dto);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _accountService.GetAllAsync(userId!, paginationParams, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _accountService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateAccountDto dto)
    {
        var result = await _accountService.UpdateAsync(id, dto);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _accountService.DeleteAsync(id);
        return HandleResult(result);
    }
}
