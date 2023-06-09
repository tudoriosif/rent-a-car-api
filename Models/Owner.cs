using Microsoft.AspNetCore.Identity;

namespace RentACarAPI.Models
{
    public class Owner : IdentityUser
    {
        public ICollection<Car> Cars { get; set; }

        public ICollection<RentingEvent> RentingEvents { get; set;}
    }
}
