using GateMonitor.Models.Requests;
using GateMonitor.Services;
using Microsoft.AspNetCore.Mvc;

namespace GateMonitor.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var result = _userService.ValidateUser(request.Username, request.Password).Result;
            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }

            return Ok(result);
        }
    }
}
