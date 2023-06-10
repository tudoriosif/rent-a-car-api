using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.Services;

namespace RentACarAPI.Controllers.User
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("past-events")]
        public async Task<IActionResult> PastEvents()
        {
            var result = await _userService.GetPastRentEventsAsync();

            if (!result.isSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("current-event")]
        public async Task<IActionResult> CurrentEvent()
        {
            var result = await _userService.GetCurrentRentEventAsync();

            if (!result.isSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [Authorize]
        [HttpGet("planned-events")]
        public async Task<IActionResult> PlannedEvents()
        {
            var result = await _userService.GetPlannedEventsAsync();

            if (!result.isSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("planned-event/{carId}")]
        public async Task<IActionResult> PlannedEventByCar(int carId)
        {
            var result = await _userService.GetPlannedEventAsync(carId);

            if (!result.isSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
