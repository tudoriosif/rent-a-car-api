using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace RentACarAPI.Models
{
    public class Owner : IdentityUser
    {
        public ICollection<Car> Cars { get; set; }

        [JsonIgnore]
        public ICollection<RentingEvent> RentingEvents { get; set;}
    }
}
