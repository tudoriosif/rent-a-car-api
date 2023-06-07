using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Models
{
    public enum CarTypeEnum
    {
        Budget = 0,
        Common = 1,
        Luxury = 2,
    }

    public class CarType
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public string Type { get; set; }
    }
}
