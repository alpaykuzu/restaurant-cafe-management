using Core.Dtos.RestaurantDtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager, Waiter, Kitchen, Cashier")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }
        [HttpGet("get-restaurant-by-user-id")]
        public async Task<IActionResult> GetRestaurantById()
        {
            return Ok(await _restaurantService.GetRestaurantByUserIdAsync());
        }
        [HttpGet("get-all-restaurants")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRestaurants()
        {
            return Ok(await _restaurantService.GetAllRestaurantsAsync());
        }
        [HttpPost("create-restaurant")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantRequestDto req)
        {
            return Ok(await _restaurantService.CreateRestaurantAsync(req));
        }
        [HttpPut("update-restaurant")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRestaurant([FromBody] UpdateRestaurantRequestDto req)
        {
            return Ok(await _restaurantService.UpdateRestaurantAsync(req));
        }   
        [HttpDelete("delete-restaurant/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            return Ok(await _restaurantService.DeleteRestaurantAsync(id));
        }
    }
}
