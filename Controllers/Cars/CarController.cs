using Microsoft.AspNetCore.Mvc;
using RentACarAPI.Services;

namespace RentACarAPI.Controllers.Cars
{

    [Route("api/cars")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CarRequest carRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("The request is not valid!");
            }

            var result = await _carService.AddCar(carRequest);

            if(!result.isSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _carService.GetAllCars();

            if(!result.isSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _carService.GetCarById(id);

            if (result.Car == null) return NotFound(result);

            if (!result.isSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _carService.DeleteCar(id);

            if (result.Car == null) return NotFound(result);

            if (!result.isSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]UpdateCarRequest updateCarRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("The request is not valid!");
            }

            var result = await _carService.UpdateCar(id, updateCarRequest);

            if (result.Car == null)
            {
                return NotFound(result);
            }

            if (!result.isSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("past-events/{id}")]
        public async Task<IActionResult> PastEvents(int id)
        {
            var result = await _carService.GetPastEvents(id);

            if (!result.isSuccess)
            {
                if(result.Car == null)
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("future-events/{id}")]
        public async Task<IActionResult> FutureEvents(int id)
        {
            var result = await _carService.GetFutureEvents(id);

            if (!result.isSuccess)
            {
                if (result.Car == null)
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
