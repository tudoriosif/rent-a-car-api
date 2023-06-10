using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Contexts;
using RentACarAPI.Controllers.Rentings;
using RentACarAPI.Models;

namespace RentACarAPI.Services
{

    public interface IRentingService
    {
        Task<RentingResponse> PlanRentEvent(PlanRentingRequest model);

        Task<RentingResponse> StartRent(int carId);

        Task<RentingResponse> FinishRent(int carId);

        Task<RentingResponse> CancelPlannedRent(int carId);
    }

    public class RentingService: IRentingService
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<Owner> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public RentingService(DataContext dataContext, UserManager<Owner> userManager, IHttpContextAccessor contextAccessor)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        private Owner GetCurrentOwner()
        {
            return _userManager.GetUserAsync(_contextAccessor.HttpContext.User).Result;
        }

        public async Task<RentingResponse> PlanRentEvent(PlanRentingRequest model)
        {
            var owner = GetCurrentOwner();
            if(owner == null)
            {
                return new RentingResponse
                {
                    Message = "User not found",
                    isSuccess = false
                };
            }

            var car = await _dataContext.Cars.FindAsync(model.CarId);

            if(car == null)
            {
                return new RentingResponse
                {
                    Message = "Car not found",
                    isSuccess = false,
                    Owner = owner
                };
            }

            if (!car.isAvailable)
            {
                return new RentingResponse
                {
                    Message = "Car is not available",
                    isSuccess = false,
                    Car = car,
                    Owner = owner
                };
            }

            // Update car rental dates
            car.RentalStartDate = model.RentalStartDate;
            car.RentalEndDate = model.RentalEndDate;
            car.OwnerId = owner.Id;

            // Calculate costs
            var totalRentingHours = (model.RentalEndDate - model.RentalStartDate).TotalHours;
            var totalCost = totalRentingHours * car.Price;

            // Create rating event
            var rentingEvent = new RentingEvent
            {
                CarId = model.CarId,
                RentalStartDate = model.RentalStartDate,
                RentalEndDate = model.RentalEndDate,
                OwnerId = owner.Id,
                PricePerHour = car.Price,
                TotalRentingHours = totalRentingHours,
                TotalCost = totalCost
            };

            try
            {
                _dataContext.RentingEvents.Add(rentingEvent);
                await _dataContext.SaveChangesAsync();
            }catch(Exception ex)
            {
                return new RentingResponse
                {
                    Message = "Couldn't create RentingEvent",
                    isSuccess = false,
                    Car = car,
                    Owner = owner,
                    Errors = new List<string>()
                    {
                        ex.Message
                    }
                };
            }

            return new RentingResponse
            {
                Message = "Renting planned successfully",
                isSuccess = true,
                RentingEvent = rentingEvent
            };
        }

        public async Task<RentingResponse> StartRent(int carId)
        {
            var owner = GetCurrentOwner();
            if (owner == null)
            {
                return new RentingResponse
                {
                    Message = "User not found",
                    isSuccess = false
                };
            }

            var rentalStartDate = DateTime.Now;

            var currentlyRented = await _dataContext.Cars
                .Where(c => c.OwnerId == owner.Id && c.RentalStartDate <= rentalStartDate && c.RentalEndDate == null)
                .ToListAsync();

            if (currentlyRented.Any())
            {
                return new RentingResponse
                {
                    Message = "You already has a rented car!",
                    isSuccess = false,
                    Owner = owner,
                };
            }

            var car = await _dataContext.Cars.FindAsync(carId);

            if (car == null)
            {
                return new RentingResponse
                {
                    Message = "Car not found",
                    isSuccess = false,
                    Owner = owner
                };
            }

            if(!car.isAvailable)
            {
                return new RentingResponse
                {
                    Message = "Car is not available",
                    isSuccess = false,
                    Car = car,
                    Owner = owner
                };
            }

            // Update car rental
            car.RentalStartDate = rentalStartDate;
            car.RentalEndDate = null;
            car.OwnerId = owner.Id;

            // Create rating event
            var rentingEvent = new RentingEvent
            {
                CarId = carId,
                RentalStartDate = rentalStartDate,
                RentalEndDate = null,
                OwnerId = owner.Id,
                PricePerHour = car.Price,
            };

            try
            {
                _dataContext.RentingEvents.Add(rentingEvent);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new RentingResponse
                {
                    Message = "Couldn't create RentingEvent",
                    isSuccess = false,
                    Car = car,
                    Owner = owner,
                    Errors = new List<string>()
                    {
                        ex.Message
                    }
                };
            }

            return new RentingResponse
            {
                Message = "Renting started successfully",
                isSuccess = true,
                RentingEvent = rentingEvent
            };
        }

        public async Task<RentingResponse> FinishRent(int carId)
        {
            var owner = GetCurrentOwner();
            if (owner == null)
            {
                return new RentingResponse
                {
                    Message = "User not found",
                    isSuccess = false
                };
            }

            var car = await _dataContext.Cars.FindAsync(carId);

            if (car == null)
            {
                return new RentingResponse
                {
                    Message = "Car not found",
                    isSuccess = false,
                    Owner = owner
                };
            }

            if (car.isAvailable)
            {
                return new RentingResponse
                {
                    Message = "You don't have an active rent to finish on this car!",
                    isSuccess = false,
                    Car = car,
                    Owner = owner
                };
            }

            var rentalEndDate = DateTime.Now;

            car.RentalEndDate = rentalEndDate;
            car.OwnerId = null;

            var rentingEvent = await _dataContext.RentingEvents
                .Include(re => re.Car)
                .Where(re => re.CarId == carId && re.OwnerId == owner.Id && re.RentalEndDate == null && re.RentalStartDate != null)
                .SingleOrDefaultAsync();

            if (rentingEvent == null)
            {
                return new RentingResponse
                {
                    Message = "Renting event not found or already has been finished!",
                    isSuccess = true,
                    Owner = owner,
                    Car = car
                };
            }

            rentingEvent.RentalEndDate = rentalEndDate;
            var totalHours = (rentalEndDate - (DateTime)rentingEvent.RentalStartDate).TotalHours;
            rentingEvent.TotalRentingHours = totalHours;
            rentingEvent.TotalCost = rentingEvent.PricePerHour * totalHours;

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return new RentingResponse
                {
                    Message = "Couldn't update the renting",
                    isSuccess = false,
                    Car = car,
                    Owner = owner,
                    Errors = new List<string>()
                    {
                        ex.Message
                    }
                };
            }

            return new RentingResponse
            {
                Message = "Renting finished successuflly",
                isSuccess = true,
                RentingEvent = rentingEvent
            };
        }

        public async Task<RentingResponse> CancelPlannedRent(int carId)
        {
            var owner = GetCurrentOwner();
            if (owner == null)
            {
                return new RentingResponse
                {
                    Message = "User not found",
                    isSuccess = false
                };
            }

            var car = await _dataContext.Cars.FindAsync(carId);

            if (car == null)
            {
                return new RentingResponse
                {
                    Message = "Car not found",
                    isSuccess = false,
                    Owner = owner
                };
            }

            var rentingEvent = await _dataContext.RentingEvents
                .Include(re => re.Car)
                .Where(re => re.CarId == carId && re.OwnerId == owner.Id && re.RentalStartDate > DateTime.Now && re.RentalEndDate != null)
                .SingleOrDefaultAsync();

            if (rentingEvent == null)
            {
                return new RentingResponse
                {
                    Message = "Planned event not found or already started!",
                    isSuccess = false,
                    Owner = owner,
                    Car = car
                };
            }

            rentingEvent.RentalStartDate = null;
            rentingEvent.RentalEndDate = null;
            rentingEvent.TotalCost = 0;
            rentingEvent.TotalRentingHours = 0;

            car.RentalStartDate = null;
            car.RentalEndDate = null;
            car.OwnerId = null;

            try
            {
                await _dataContext.SaveChangesAsync();
            }catch(Exception ex) {
                return new RentingResponse
                {
                    Message = "Couldn't update the renting",
                    isSuccess = false,
                    Car = car,
                    Owner = owner,
                    Errors = new List<string>()
                    {
                        ex.Message
                    }
                };
            }

            return new RentingResponse
            {
                Message = "Planned rent cancelled successfully",
                isSuccess = true,
                RentingEvent = rentingEvent
            };
        }
    }
}
