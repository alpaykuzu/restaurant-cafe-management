using Core.Dtos.PaymentDtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpPost("make-payment")]
        [Authorize(Roles = "Manager, Cashier")]
        public async Task<IActionResult> MakePayment([FromBody] CreatePaymentRequestDto req)
        {
            return Ok(await _paymentService.MakePaymentAsync(req));
        }
        [HttpGet("get-payment-by-id/{paymentId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            return Ok(await _paymentService.GetPaymentByIdAsync(paymentId));
        }
        [HttpGet("get-payment-by-order-id/{orderId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetPaymentsByOrderId(int orderId)
        {
            return Ok(await _paymentService.GetPaymentByOrderIdAsync(orderId));
        }
        [HttpGet("get-payments-by-restaurant-id")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetPaymentsByRestaurantId()
        {
            return Ok(await _paymentService.GetPaymentsByRestaurantIdAsync());
        }
    }
}
