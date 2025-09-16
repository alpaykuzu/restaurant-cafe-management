using Core.Dtos.InventoryItemDtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")]
    public class InventoryItemController : ControllerBase
    {
        private readonly IInventoryItemService _inventoryItemService;
        public InventoryItemController(IInventoryItemService inventoryItemService)
        {
            _inventoryItemService = inventoryItemService;
        }
        [HttpGet("get-all-inventory-items-by-restaurant-id/{restaurantId}")]
        public async Task<IActionResult> GetAllInventoryByRestaurantId(int restaurantId)
        {
            return Ok(await _inventoryItemService.GetAllInventoryByRestaurantIdAsync(restaurantId));
        }
        [HttpGet("get-inventory-item-by-id/{id}")]
        public async Task<IActionResult> GetInventoryItemById(int id)
        {
            return Ok(await _inventoryItemService.GetInventoryItemByIdAsync(id));
        }
        [HttpPut("update-inventory-item")]
        public async Task<IActionResult> UpdateInventoryItem([FromBody] UpdateInventoryItemRequestDto req)
        {
            return Ok(await _inventoryItemService.UpdateInventoryItemAsync(req));
        }
        [HttpPut("update-stock-level/{id}/{newStockLevel}")]
        public async Task<IActionResult> UpdateStockLevel(int id, int newStockLevel)
        {
            return Ok(await _inventoryItemService.UpdateStockLevelAsync(id, newStockLevel));
        }
        [HttpPost("create-inventory-item")]
        public async Task<IActionResult> CreateInventoryItem([FromBody] CreateInventoryItemRequestDto req)
        {
            return Ok(await _inventoryItemService.CreateInventoryItemAsync(req));
        }
        [HttpDelete("delete-inventory-item/{id}")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            return Ok(await _inventoryItemService.DeleteInventoryItemAsync(id));
        }
    }
}
