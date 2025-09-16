using Core.Dtos.MenuItemDtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager, Waiter, Kitchen, Cashier")]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;
        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }
        [HttpGet("get-all-menu-items-by-restaurant-id")]
        public async Task<IActionResult> GetAllMenuItemsByRestaurantId()
        {
            return Ok(await _menuItemService.GetAllMenuItemsByRestaurantIdAsync());
        }
        [HttpGet("get-menu-items-by-category-id/{categoryId}")]
        public async Task<IActionResult> GetMenuItemsByCategoryId(int categoryId)
        {
            return Ok(await _menuItemService.GetMenuItemsByCategoryIdAsync(categoryId));
        }
        [HttpGet("get-menu-item-by-id/{id}")]
        public async Task<IActionResult> GetMenuItemById(int id)
        {
            return Ok(await _menuItemService.GetMenuItemByIdAsync(id));
        }
        [Authorize(Roles = "Manager")]
        [HttpPost("create-menu-item")]
        public async Task<IActionResult> CreateMenuItem([FromBody] CreateMenuItemRequestDto req)
        {
            return Ok(await _menuItemService.CreateMenuItemAsync(req));
        }
        [HttpPut("update-menu-item")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateMenuItem([FromBody] UpdateMenuItemRequestDto req)
        {
            return Ok(await _menuItemService.UpdateMenuItemAsync(req));
        }
        [HttpPatch("update-price/{id},{newPrice}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdatePrice(int id, int newPrice)
        {
            return Ok(await _menuItemService.UpdatePriceAsync(id, newPrice));
        }
        [HttpDelete("delete-menu-item/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            return Ok(await _menuItemService.DeleteMenuItemAsync(id));
        }
    }
}
