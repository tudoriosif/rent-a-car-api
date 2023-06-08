using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required, MaxLength(25)]
        public string? Model { get; set; }

        [Required]
        public CarType? CarType { get; set; }

        [Required]
        public int Odometer { get; set; } = 0;

        [Required]
        public double Price { get; set; } = 100.0;

        [Required]
        public int Year { get; set; }

        public int PositionId { get; set; }
        public Position? Position { get; set; }

        public string? OwnerId { get; set; }
        public Owner? Owner { get; set; }
    }
}
