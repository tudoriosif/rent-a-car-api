using RentACarAPI.Models;

namespace RentACarAPI.Controllers.Cars
{
    public class CarResponse
    {
        public string Message { get; set; }

        public bool isSuccess { get; set; }

        public IEnumerable<Car>? Cars { get; set; }

        public Car? Car { get; set; }

        public IEnumerable<string>? Errors { get; set; }
    }
}
