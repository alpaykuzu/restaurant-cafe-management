using Core.Dtos.CategoryDtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager, Waiter, Kitchen, Cashier")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet("get-all-category-by-restaurant-id")]
        public async Task<IActionResult> GetAllCategoryByRestaurantId(int restaurantId)
        {
            return Ok(await _categoryService.GetAllCategoryByRestaurantIdAsync());
        }
        [HttpGet("get-category-by-id/{categoryId}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            return Ok(await _categoryService.GetCategoryByIdAsync(categoryId));
        }
        [HttpPost("create-category")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto req)
        {
            return Ok(await _categoryService.CreateCategoryAsync(req));
        }
        [HttpPut("update-category")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryRequestDto req)
        {
            return Ok(await _categoryService.UpdateCategoryAsync(req));
        }
        [HttpDelete("delete-category/{categoryId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            return Ok(await _categoryService.DeleteCategoryAsync(categoryId));
        }
    }
}
