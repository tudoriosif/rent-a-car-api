using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentACarAPI.Contexts;
using RentACarAPI.Controllers.Auth;
using RentACarAPI.Controllers.User;
using RentACarAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RentACarAPI.Services
{
    public interface IUserService
    {
        Task<UserResponse> RegisterUserAsync(RegisterRequest model);

        Task<UserResponse> LoginUserAsync(LoginRequest model);

        Task<UserResponse> GetPlannedEventAsync(int carId);

        Task<UserResponse> GetPlannedEventsAsync();

        Task<UserResponse> GetCurrentRentEventAsync();

        Task<UserResponse> GetPastRentEventsAsync();
    }

    public class UserService : IUserService
    {

        private UserManager<Owner> _userManager;
        private IConfiguration _config;
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService (DataContext dataContext, UserManager<Owner> userManager, IConfiguration config, IHttpContextAccessor contextAccessor)
        {
            _dataContext = dataContext;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _config = config;
        }

        private Owner GetCurrentOwner()
        {
            return _userManager.GetUserAsync(_contextAccessor.HttpContext.User).Result;
        }

        public async Task<UserResponse> RegisterUserAsync(RegisterRequest model)
        {
            if (model == null)
            {
                throw new NullReferenceException("Register model is null");
            }

            if (model.Password != model.ConfirmPassword) {

                return new UserResponse
                {
                    Message = "Confirm password doesn't match the password",
                    isSuccess = false,
                };
            }

            var identityUser = new Owner
            {
                Email = model.Email,
                UserName = model.UserName,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (!result.Succeeded)
            {
                return new UserResponse
                {
                    Message = "User did not create",
                    isSuccess = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return new UserResponse
            {
                Message = "User created successfully",
                isSuccess = true,
            };
        }

        public async Task<UserResponse> LoginUserAsync(LoginRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new UserResponse
                {
                    Message = "User doesn't exists!",
                    isSuccess = false,
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return new UserResponse
                {
                    Message = "Password doesn't match",
                    isSuccess = false,
                };
            }

            var claims = new[]
            {
                new Claim("Email", model.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var configKey = _config.GetValue<string>("AuthSettings:Key");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("AuthSettings:Key")));

            var token = new JwtSecurityToken(
                issuer: _config.GetValue<string>("AuthSettings:Issuer"),
                audience: _config.GetValue<string>("AuthSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserResponse
            {
                Message = tokenAsString,
                isSuccess = true
            };
        }

        public async Task<UserResponse> GetPlannedEventAsync(int carId)
        {
            var owner = GetCurrentOwner();
            if (owner == null)
            {
                return new UserResponse
                {
                    Message = "You are not logged in!",
                    isSuccess = false
                };
            }

            var car = await _dataContext.Cars.SingleOrDefaultAsync(c => c.Id == carId);

            if(car == null)
            {
                return new UserResponse
                {
                    Message = "That car doesn't exists",
                    isSuccess = false,
                };
            }

            var currentDateTime = DateTime.Now;

            var rentingEvent = await _dataContext.RentingEvents
                .Include(re => re.Car)
                .Where(re => re.CarId == carId && re.OwnerId == owner.Id)
                .Where(re => re.Car.OwnerId == owner.Id)
                .Where(re => re.RentalStartDate > currentDateTime && re.RentalEndDate != null)
                .OrderBy(re => re.RentalStartDate)
                .FirstOrDefaultAsync();

            if(rentingEvent == null)
            {
                return new UserResponse
                {
                    Message = "No planned event for this car!",
                    isSuccess = false,
                    Owner = owner
                };
            }

            return new UserResponse
            {
                Message = "Planned event received successfully",
                isSuccess = true,
                RentingEvent = rentingEvent
            };
        }

        public async Task<UserResponse> GetPlannedEventsAsync()
        {
            var owner = GetCurrentOwner();
            if (owner == null)
            {
                return new UserResponse
                {
                    Message = "You are not logged in!",
                    isSuccess = false
                };
            }

            var currentDateTime = DateTime.Now;

            var rentingEvents = await _dataContext.RentingEvents
                .Include(re => re.Car)
                .Where(re => re.OwnerId == owner.Id)
                .Where(re => re.RentalStartDate > currentDateTime && re.RentalEndDate != null)
                .OrderBy(re => re.RentalStartDate)
                .ToListAsync();

            if (rentingEvents == null)
            {
                return new UserResponse
                {
                    Message = "No planned events for this car!",
                    isSuccess = false,
                    Owner = owner
                };
            }

            return new UserResponse
            {
                Message = "Planned event received successfully",
                isSuccess = true,
                RentingEvents = rentingEvents
            };
        }

        public async Task<UserResponse> GetCurrentRentEventAsync()
        {
            var owner = GetCurrentOwner();
            if (owner == null)
            {
                return new UserResponse
                {
                    Message = "You are not logged in!",
                    isSuccess = false
                };
            }

            var currentTime = DateTime.Now;

            var rentingEvent = await _dataContext.RentingEvents
                .Include(re => re.Car)
                .Include(re => re.Owner)
                .Where(re => re.OwnerId == owner.Id)
                .Where(re => re.Car.OwnerId == owner.Id && re.RentalStartDate <= currentTime && re.RentalEndDate == null)
                .FirstOrDefaultAsync();

            if (rentingEvent == null)
            {
                return new UserResponse
                {
                    Message = "You don't have an active rent",
                    isSuccess = false,
                    Owner = owner
                };
            }

            return new UserResponse
            {
                Message = "Rent event received successfully",
                isSuccess = true,
                RentingEvent = rentingEvent
            };
        }

        public async Task<UserResponse> GetPastRentEventsAsync()
        {
            var owner = GetCurrentOwner();
            if (owner == null)
            {
                return new UserResponse
                {
                    Message = "You are not logged in!",
                    isSuccess = false
                };
            }

            var currentDateTime = DateTime.Now;

            var rentingEvents = await _dataContext.RentingEvents
                .Include(re => re.Car)
                .Where(re => re.OwnerId == owner.Id)
                .Where(re => re.RentalStartDate != null && re.RentalEndDate != null && re.RentalEndDate < currentDateTime)
                .OrderByDescending(re => re.RentalEndDate)
                .ToListAsync();

            if (rentingEvents == null)
            {
                return new UserResponse
                {
                    Message = "No past planned events for this car!",
                    isSuccess = false,
                    Owner = owner
                };
            }

            return new UserResponse
            {
                Message = "Past planned event received successfully",
                isSuccess = true,
                RentingEvents = rentingEvents
            };
        }
          
    }
}
