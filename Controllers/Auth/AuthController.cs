using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.Controllers.User;
using RentACarAPI.Services;

namespace RentACarAPI.Controllers.Auth
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("The request is not valid!"); // 400
            }

            var result = await _userService.RegisterUserAsync(model);

            if(!result.isSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result); // 200
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginRequest model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Login data are not valid!"); // 400
            }

            var result = await _userService.LoginUserAsync(model);

            if (!result.isSuccess)
            {
                return BadRequest(result); // 400
            }

            return Ok(result);
        }
    }
}
