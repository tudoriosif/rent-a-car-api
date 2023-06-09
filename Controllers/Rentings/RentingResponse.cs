using RentACarAPI.Models;

namespace RentACarAPI.Controllers.Rentings
{
    public class RentingResponse
    {
        public string Message { get; set; }

        public bool isSuccess { get; set; }

        public RentingEvent? RentingEvent { get; set; }

        public Owner? Owner { get; set; }

        public Car? Car { get; set; }

        public IEnumerable<string>? Errors { get; set; }
    }
}
