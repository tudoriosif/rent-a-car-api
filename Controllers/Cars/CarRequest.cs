using RentACarAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Controllers.Cars
{
    public class CarRequest
    {
        [Required]
        [MaxLength(25)]
        public string Model { get; set; }

        [Required]
        public string CarType { get; set; }

        public int Odometer { get; set; } = 0;

        public double Price { get; set; } = 100.0;

        [Required]
        public int Year { get; set; }

        public int PositionId { get; set; }

        public string? OwnerId { get; set; }
    }
}
