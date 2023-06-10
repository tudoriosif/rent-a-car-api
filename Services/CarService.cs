using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Contexts;
using RentACarAPI.Controllers.Cars;
using RentACarAPI.Models;

namespace RentACarAPI.Services
{
    public interface ICarService
    {
        Task<CarResponse> AddCar(CarRequest model);

        Task<CarResponse> GetAllCars();

        Task<CarResponse> GetCarById(int id);

        Task<CarResponse> DeleteCar(int id);

        Task<CarResponse> UpdateCar(int id, UpdateCarRequest model);

        Task<CarResponse> GetPastEvents(int id);

        Task<CarResponse> GetFutureEvents(int id);
    }

    public class CarService : ICarService
    {
        private readonly DataContext _dataContext;

        public CarService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<CarResponse> AddCar(CarRequest model)
        {
            if (model == null) { throw new NullReferenceException("Car model is null"); }

            var carType = _dataContext.CarTypes.Single(carType => carType.Type == model.CarType);

            if (carType == null) {
                return new CarResponse
                {
                    Message = "Car Type is not valid",
                    isSuccess = false
                };
            }

            var postion = new Position
            {
                Latitude = 0.0,
                Longitude = 0.0,
            };

            _dataContext.Positions.Add(postion);

            try
            {
                await _dataContext.SaveChangesAsync();
            } catch (Exception ex)
            {
                return new CarResponse
                {
                    Message = "Position couldn't be created!",
                    isSuccess = false
                };
            }



            var newCar = new Car
            {
                Model = model.Model,
                CarType = carType,
                Odometer = model.Odometer,
                Price = model.Price,
                Year = model.Year,
            };

            newCar.Position = postion;

            _dataContext.Cars.Add(newCar);

            try
            {
                await _dataContext.SaveChangesAsync();
            } catch (Exception ex)
            {
                return new CarResponse
                {
                    Message = "Car couldn't be created!",
                    isSuccess = false
                };
            }

            return new CarResponse
            {
                Message = "Car has been successfully created!",
                isSuccess = true,
                Car = newCar
            };
        }
    
        public async Task<CarResponse> GetAllCars()
        {
            IEnumerable<Car> cars;

            try
            {
                cars = await _dataContext.Cars
                    .Include(car => car.Position)
                    .Include(car => car.CarType)
                    .Include(car => car.Owner)
                    .ToListAsync();

            }catch (Exception ex)
            {
                return new CarResponse
                {
                    Message = "Couldn't get the cars",
                    isSuccess = false
                };
            }

            return new CarResponse
            {
                Message = "Cars retrieved successfully",
                isSuccess = true,
                Cars = cars
            };
        }

        public async Task<CarResponse> GetCarById(int id)
        {
            Car car;

            try
            {
                car = _dataContext.Cars
                    .Include(car => car.Position)
                    .Include(car => car.CarType)
                    .Include(car => car.Owner)
                    .Include(car => car.RentingEvents)
                    .Single(c => c.Id == id);
            }catch (Exception ex)
            {
                return new CarResponse
                {
                    Message = "Couldn't get the car!",
                    isSuccess = false,
                    Errors = new List<string>
                    {
                        ex.ToString()
                    }
                };
            }

            return new CarResponse
            {
                Message = "Car has been retrieved!",
                isSuccess = true,
                Car = car
            };
        }

        public async Task<CarResponse> DeleteCar(int id)
        {
            var car = await _dataContext.Cars.FindAsync(id);

            if (car == null)
            {
                return new CarResponse
                {
                    Message = "Couldn't find the car!",
                    isSuccess = false,
                };
            }

            try
            {
                _dataContext.Cars.Remove(car);
                await _dataContext.SaveChangesAsync();
            }catch(Exception ex)
            {
                return new CarResponse
                {
                    Message = "Couldn't delete the car!",
                    isSuccess = false,
                    Errors = new List<string>()
                    {
                        ex.ToString()
                    }
                };
            }

            return new CarResponse
            {
                Message = "Car has been deleted!",
                isSuccess = true,
                Car = car
            };
        }

        public async Task<CarResponse> UpdateCar(int id, UpdateCarRequest model)
        {
            Car car = _dataContext.Cars.Single(c => c.Id == id);
            
            if (car == null)
            {
                return new CarResponse
                {
                    Message = "Couldn't find the car",
                    isSuccess = false
                };
            }

            // Update position if necessary
            Position position = _dataContext.Positions.Single(p => p.Id == car.PositionId);

            if (position == null)
            {
                return new CarResponse
                {
                    Message = "Couldn't find the position",
                    isSuccess = false
                };
            }

            position.Longitude = model.Longitude;
            position.Latitude = model.Latitude;

            car.Price = model.Price;
            car.Odometer = model.Odometer;
            car.OwnerId = model.OwnerId;

            try
            {
                await _dataContext.SaveChangesAsync();
            }catch (Exception ex)
            {
                return new CarResponse
                {
                    Message = "Couldn't save the changes",
                    isSuccess = false,
                };
            }

            return new CarResponse
            {
                Message = "Car updated!",
                isSuccess = true,
                Car = car
            };
        }

        public async Task<CarResponse> GetPastEvents(int id)
        {
            var currentTime = DateTime.Now;

            var car = await _dataContext.Cars
                .Include(c => c.CarType)
                .Include(c => c.RentingEvents.Where(re => re.RentalEndDate < currentTime && re.RentalStartDate != null))
                    .ThenInclude(re => re.Owner)
                .SingleOrDefaultAsync(car => car.Id == id);

            if(car == null)
            {
                return new CarResponse
                {
                    Message = "Car couldn't be found!",
                    isSuccess = false
                };
            }

            if (!car.RentingEvents.Any()) 
            {
                return new CarResponse
                {
                    Message = "There are no past renting events!",
                    isSuccess = false,
                    Car = car
                };
            }

            return new CarResponse
            {
                Message = "Past renting events received successfully!",
                isSuccess = true,
                RentingEvents = car.RentingEvents
            };
        }

        public async Task<CarResponse> GetFutureEvents(int id)
        {
            var currentTime = DateTime.Now;
            var car = await _dataContext.Cars
                .Include(c => c.CarType)
                .Include(c => c.RentingEvents.Where(re => re.RentalStartDate > currentTime && re.RentalStartDate != null))
                    .ThenInclude(re => re.Owner)
                .SingleOrDefaultAsync(car => car.Id == id);

            if (car == null)
            {
                return new CarResponse
                {
                    Message = "Car couldn't be found!",
                    isSuccess = false
                };
            }

            if (!car.RentingEvents.Any())
            {
                return new CarResponse
                {
                    Message = "There are no future renting events!",
                    isSuccess = false,
                    Car = car
                };
            }

            return new CarResponse
            {
                Message = "Future renting events received successfully!",
                isSuccess = true,
                RentingEvents = car.RentingEvents
            };
        }
    }
}
