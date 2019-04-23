using System.ComponentModel.DataAnnotations;

namespace dotnet_webapi_jwt_auth.Dtos
{
    public class UserRegistrationDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "You must specify a password with a minimum 6 characters")]
        public string Password { get; set; }
    }
}