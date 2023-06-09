using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentACarAPI.Services;

namespace RentACarAPI.Controllers.Rentings
{
    [Route("api/rent")]
    [ApiController]
    public class RentingController : ControllerBase
    {
        private IRentingService _rentingService;

        public RentingController(IRentingService rentingService)
        {
            _rentingService = rentingService;
        }

        [Authorize]
        [HttpPost("plan")]
        public async Task<IActionResult> PlanRent([FromBody]PlanRentingRequest planRentingRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RentingResponse
                {
                    Message = "Model is not valid!",
                    isSuccess = false
                });
            }

            if(planRentingRequest.RentalStartDate <= DateTime.Now)
            {
                return BadRequest(new RentingResponse
                {
                    Message = "You can't plan a rent in the past",
                    isSuccess = false
                });
            }

            if((planRentingRequest.RentalEndDate - planRentingRequest.RentalStartDate).TotalHours < 1)
            {
                return BadRequest(new RentingResponse
                {
                    Message = "Not enough hours planned!",
                    isSuccess = false
                });
            }

            var result = await _rentingService.PlanRentEvent(planRentingRequest);

            if(!result.isSuccess)
            {
                if (result.Car == null || result.Owner == null) return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{carId}")]
        public async Task<IActionResult> StartRent(int carId)
        {

            var result = await _rentingService.StartRent(carId);

            if (!result.isSuccess)
            {
                if (result.Car == null || result.Owner == null) return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{carId}/finish")]
        public async Task<IActionResult> FinishRent(int carId)
        {

            var result = await _rentingService.FinishRent(carId);

            if (!result.isSuccess)
            {
                if (result.Car == null || result.Owner == null) return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{carId}/cancel")]
        public async Task<IActionResult> CancelRent(int carId)
        {

            var result = await _rentingService.CancelPlannedRent(carId);

            if (!result.isSuccess)
            {
                if (result.Car == null || result.Owner == null) return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
