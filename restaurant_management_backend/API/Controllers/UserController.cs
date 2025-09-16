using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, Manager")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("get-all-user")]
        public async Task<IActionResult> GetAllUser()
        {
            return Ok(await _userService.GetAllUserAsync());
        }
        [HttpGet("get-user-by-id")]
        public async Task<IActionResult> GetUserById()
        {
            return Ok(await _userService.GetUserByIdAsync());
        }
        [HttpGet("get-managers")]
        public async Task<IActionResult> GetManagers()
        {
            return Ok(await _userService.GetManagersAsync());
        }
    }
}
