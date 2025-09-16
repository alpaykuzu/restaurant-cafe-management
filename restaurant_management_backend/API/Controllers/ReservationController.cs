using Core.Dtos.ReservationDtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }
        [HttpPost("create-reservation")]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequestDto req)
        {
            return Ok(await _reservationService.CreateReservationAsync(req));
        }
        [HttpPut("update-reservation")]
        public async Task<IActionResult> UpdateReservation([FromBody] UpdateReservationRequestDto req)
        {
            return Ok(await _reservationService.UpdateReservationAsync(req));
        }
        [HttpDelete("cancel-reservation/{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            return Ok(await _reservationService.CancelReservationAsync(id));
        }
        [HttpGet("get-reservations-by-restaurant/{restaurantId}")]
        public async Task<IActionResult> GetReservationsByRestaurant(int restaurantId)
        {
            return Ok(await _reservationService.GetReservationsByRestaurantIdAsync(restaurantId));
        }
        [HttpGet("get-reservations-by-customer/{customerName}")]
        public async Task<IActionResult> GetReservationsByCustomer(string customerName)
        {
            return Ok(await _reservationService.GetReservationsByCustomerAsync(customerName));
        }
    }
}
