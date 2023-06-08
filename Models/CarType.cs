using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Models
{
    public enum CarTypeEnum
    {
        Budget = 1,
        Common = 2,
        Luxury = 3,
    }

    public class CarType
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public string Type { get; set; }
    }
}
