using Core.Dtos.InventoryTransactionDtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Manager")]
    [ApiController]
    public class InventoryTransactionController : ControllerBase
    {
        private readonly IInventoryTransactionService _inventoryTransactionService;
        public InventoryTransactionController(IInventoryTransactionService inventoryTransactionService)
        {
            _inventoryTransactionService = inventoryTransactionService;
        }
        [HttpGet("get-all-transactions-by-restaurant/{restaurantId}")]
        public async Task<IActionResult> GetAllInventoryTransactionByRestaurantIdAsync(int restaurantId)
        {
            return Ok(await _inventoryTransactionService.GetAllInventoryTransactionByRestaurantIdAsync(restaurantId));
        }
        [HttpGet("get-transactions-by-employee/{employeeId}")]
        public async Task<IActionResult> GetInventoryTransactionByEmployeeAsync(int employeeId)
        {
            return Ok(await _inventoryTransactionService.GetInventoryTransactionByEmployeeAsync(employeeId));
        }
        [HttpPost("create-transaction")]
        public async Task<IActionResult> CreateInventoryTransactionAsync([FromBody] CreateInventoryTransactionRequestDto req)
        {
            return Ok(await _inventoryTransactionService.CreateInventoryTransactionAsync(req));
        }
        [HttpDelete("delete-transaction/{id}")]
        public async Task<IActionResult> DeleteInventoryTransactionAsync(int id)
        {
            return Ok(await _inventoryTransactionService.DeleteInventoryTransactionAsync(id));
        }
    }
}
