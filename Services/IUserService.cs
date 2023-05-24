using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RentACarAPI.Controllers.Auth;
using RentACarAPI.Controllers.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RentACarAPI.Services
{
    public interface IUserService
    {
        Task<UserResponse> RegisterUserAsync(RegisterRequest model);

        Task<UserResponse> LoginUserAsync(LoginRequest model);
    }

    public class UserService : IUserService
    {

        private UserManager<IdentityUser> _userManager;
        private IConfiguration _config;

        public UserService (UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
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

            var identityUser = new IdentityUser
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
    }
}
