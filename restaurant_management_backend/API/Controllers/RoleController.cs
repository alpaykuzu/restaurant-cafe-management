using Core.Dtos.RoleDtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager, Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleservice;
        public RoleController(IRoleService roleservice)
        {
            _roleservice = roleservice;
        }
        [HttpGet("get-all-roles-by-restaurant-id")]
        public async Task<IActionResult> GetAllRolesByRestaurantId()
        {
            return Ok(await _roleservice.GetAllRolesByRestaurantIdAsync());
        }
        [HttpGet("get-role-by-id/{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            return Ok(await _roleservice.GetRoleByIdAsync(id));
        }
        [HttpGet("get-all-roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            return Ok(await _roleservice.GetAllRoleAsync());
        }
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestDto req)
        {
            return Ok(await _roleservice.CreateRoleAsync(req));
        }
        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateRole(UpdateRoleRequestDto req)
        {
            return Ok(await _roleservice.UpdateRoleAsync(req));
        }
        [HttpDelete("delete-role")]
        public async Task<IActionResult> DeleteRole([FromBody] DeleteRoleRequestDto req)
        {
            return Ok(await _roleservice.DeleteRoleAsync(req));
        }
    }
}
