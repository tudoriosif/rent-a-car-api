using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Controllers.User
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; }

        [Required]
        [StringLength (50, MinimumLength = 5)]
        public string ConfirmPassword { get; set; }
        

    }
}
