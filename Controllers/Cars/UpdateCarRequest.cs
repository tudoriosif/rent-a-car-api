using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Controllers.Cars
{
    public class UpdateCarRequest
    {
        public int Odometer { get; set; }

        public double Price { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string? OwnerId { get; set; }
    }
}
