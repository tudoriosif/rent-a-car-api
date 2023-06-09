using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Controllers.Rentings
{
    public class PlanRentingRequest
    {
        [Required]
        public int CarId { get; set; }

        [Required]
        public DateTime RentalStartDate { get; set; }

        [Required]
        public DateTime RentalEndDate { get; set; }
    }
}
