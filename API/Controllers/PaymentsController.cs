using Application.Core;
using Application.DTOs.Payments;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

public class PaymentsController : BaseApiController
{
    private readonly IPaymentService _paymentService;
    private readonly IStripePaymentService _stripePaymentService;

    public PaymentsController(
        IPaymentService paymentService,
        IStripePaymentService stripePaymentService)
    {
        _paymentService = paymentService;
        _stripePaymentService = stripePaymentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePaymentDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _paymentService.CreateAsync(dto, userId!);
        return HandleResult(result);
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> ProcessStripePayment(PaymentRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _stripePaymentService.ProcessPaymentAsync(dto, userId!);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _paymentService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetByUser([FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _paymentService.GetAllByUserAsync(userId!, paginationParams, cancellationToken);
        return HandleResult(result);
    }
}
