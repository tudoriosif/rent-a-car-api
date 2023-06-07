using Microsoft.AspNetCore.Identity;

namespace RentACarAPI.Models
{
    public class Owner : IdentityUser
    {
        public List<Car> Cars { get; set; }
    }
}
