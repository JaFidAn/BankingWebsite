using Application.Core;
using Application.DTOs.Transactions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

public class TransactionsController : BaseApiController
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _transactionService.CreateAsync(userId!, dto);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _transactionService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetByAccountId(string accountId, [FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var result = await _transactionService.GetAllByAccountIdAsync(accountId, paginationParams, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetByUser([FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _transactionService.GetAllByUserIdAsync(userId!, paginationParams, cancellationToken);
        return HandleResult(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("suspicious")]
    public async Task<IActionResult> GetSuspicious()
    {
        var result = await _transactionService.GetSuspiciousAsync();
        return HandleResult(result);
    }
}
