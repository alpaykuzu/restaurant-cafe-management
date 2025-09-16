using Core.Dtos.EmployeeDtos;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpGet("get-employee-by-id/{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            return Ok(await _employeeService.GetEmployeeByIdAsync(id));
        }
        [HttpGet("get-all-employees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            return Ok(await _employeeService.GetAllEmployeesAsync());
        }
        [HttpGet("get-employees-by-restaurant-id/{restaurantId}")]
        public async Task<IActionResult> GetEmployeesByRestaurantId(int restaurantId)
        {
            return Ok(await _employeeService.GetEmployeesByRestaurantId(restaurantId));
        }
        [HttpGet("get-employees-own-restaurant")]
        public async Task<IActionResult> GetEmployeesOwnRestaurant()
        {
            return Ok(await _employeeService.GetEmployeesOwnRestaurant());
        }
        [HttpPost("create-employee")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequestDto req)
        {
            return Ok( await _employeeService.CreateEmployeeAsync(req));
        }
        [HttpPut("update-employee")]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequestDto req)
        {
            return Ok(await _employeeService.UpdateEmployeeAsync(req));
        }
        [HttpDelete("delete-employee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            return Ok(await _employeeService.DeleteEmployeeAsync(id));
        }
    }
}
