using Core.Dtos.OrderDtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager, Waiter, Kitchen, Cashier")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderFullRequestDto req)
        {
            return Ok(await _orderService.CreateOrderAsync(req));
        }
        [HttpPut("update-order-status/{orderId}/{newStatus}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string newStatus)
        {
            return Ok(await _orderService.UpdateOrderStatusAsync(orderId, newStatus));
        }
        [HttpGet("get-all-orders-by-restaurant-id")]
        public async Task<IActionResult> GetOrdersByRestaurant()
        {
            return Ok(await _orderService.GetOrdersByRestaurantIdAsync());
        }
        [HttpGet("get-all-orders-by-daily")]
        public async Task<IActionResult> GetOrdersByDaily([FromQuery] DateTime startDate)
        {
            return Ok(await _orderService.GetOrdersByDateAsync(startDate));
        }
        [HttpGet("get-order-by-id/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            return Ok(await _orderService.GetOrderByIdAsync(id));
        }
        [HttpGet("get-orders-by-status/{status}")]
        public async Task<IActionResult> GetOrdersByStatus(string status)
        {
            return Ok(await _orderService.GetOrdersByStatusAsync(status));
        }
    }
}
