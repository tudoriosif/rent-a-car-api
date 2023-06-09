namespace RentACarAPI.Models
{
    public class RentingEvent
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public string OwnerId { get; set; }
        public Owner Owner { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }

        public double CurrentRentingHours
        {
            get
            {
                if (RentalStartDate.HasValue && RentalStartDate <= DateTime.Now) return (DateTime.Now - RentalStartDate.Value).TotalHours;
                return 0;
            }
        }

        public double CurrentRentingCost
        {
            get
            {
                if (RentalStartDate.HasValue && RentalStartDate <= DateTime.Now) return CurrentRentingHours * PricePerHour;
                return 0;
            }
        }

        public double? TotalRentingHours { get; set; }

        public double PricePerHour { get; set; }

        public double? TotalCost { get; set; }
    }
}
