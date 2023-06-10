using RentACarAPI.Models;

namespace RentACarAPI.Controllers.User
{
    public class UserResponse
    {
        public string Message { get; set; }

        public bool isSuccess { get; set; }

        public Owner? Owner { get; set; }

        public RentingEvent? RentingEvent { get; set; }

        public IEnumerable<RentingEvent>? RentingEvents { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}
