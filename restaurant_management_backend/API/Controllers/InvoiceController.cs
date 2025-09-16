using Core.Dtos.InvoiceDtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Cashier")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }
        [HttpGet("get-all-invoice-by-restaurant-id")]
        public async Task<IActionResult> GetInvoicesByRestaurant()
        {
            return Ok(await _invoiceService.GetInvoicesByRestaurantIdAsync());
        }
        [HttpGet("get-all-invoices-by-daily")]
        public async Task<IActionResult> GetOrdersByDaily([FromQuery] DateTime startDate)
        {
            return Ok(await _invoiceService.GetInvoicesByDateAsync(startDate));
        }
        [HttpGet("get-invoice-by-id/{id}")]
        public async Task<IActionResult> GetInvoice(int invoiceId)
        {
            return Ok(await _invoiceService.GetInvoiceByIdAsync(invoiceId));
        }
        [HttpPost("create-invoice/{orderId}")]
        public async Task<IActionResult> CreateInvoice(int orderId)
        {
            return Ok(await _invoiceService.CreateInvoiceAsync(orderId));
        }
    }
}
