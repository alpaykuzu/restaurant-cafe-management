using Core.Dtos.TableDtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager, Waiter, Kitchen, Cashier")]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;
        public TableController(ITableService tableService)
        {
            _tableService = tableService;
        }
        [HttpGet("get-table-by-id/{id}")]
        public async Task<IActionResult> GetTableById(int id)
        {
            return Ok(await _tableService.GetTableByIdAsync(id));
        }
        [HttpGet("get-all-tables-by-restaurant-id")]
        public async Task<IActionResult> GetAllTablesByRestaurantId()
        {
            return Ok(await _tableService.GetAllTablesByRestaurantIdAsync());
        }
        [HttpGet("get-table-count-by-restaurant-id")]
        public async Task<IActionResult> GetTableCountByRestaurantId()
        {
            return Ok(await _tableService.GetTableCountByRestaurantIdAsync());
        }
        [HttpPost("create-table")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateTable([FromBody] CreateTableRequestDto req)
        {
            return Ok(await _tableService.CreateTableAsync(req));
        }
        [HttpPut("update-table")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateTable([FromBody] UpdateTableRequestDto req)
        {
            return Ok(await _tableService.UpdateTableAsync(req));
        }
        [HttpDelete("delete-table/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            return Ok(await _tableService.DeleteTableAsync(id));
        }
        [HttpPut("update-table-status")]
        public async Task<IActionResult> UpdateTableStatus([FromBody] UpdateTableStatusRequestDto req)
        {
            return Ok(await _tableService.UpdateTableStatusByRestaurantIdAndIdAsync(req));
        }
        [HttpGet("get-table-count-by-restaurant-id-and-status/{status}")]
        public async Task<IActionResult> GetTableCountByRestaurantIdAndStatus(string status)
        {
            return Ok(await _tableService.GetTableCountByRestaurantIdAndStatusAsync(status));
        }
        [HttpGet("get-tables-by-restaurant-id-and-status/{status}")]
        public async Task<IActionResult> GetTablesByRestaurantIdAndStatus(string status)
        {
            return Ok(await _tableService.GetTablesByRestaurantIdAndStatusAsync(status));
        }
    }
}
