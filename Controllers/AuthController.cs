using System.Threading.Tasks;
using dotnet_webapi_jwt_auth.Dtos;
using dotnet_webapi_jwt_auth.Models;
using dotnet_webapi_jwt_auth.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_webapi_jwt_auth.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            this._repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserRegistrationDto userRegistrationDto)
        {
            userRegistrationDto.Username = userRegistrationDto.Username.ToLower();

            if (await _repo.UserExists(userRegistrationDto.Username))
                ModelState.AddModelError("Username", "Username is already taken");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userToCreate = new User
            {
                Username = userRegistrationDto.Username    
            };

            var createdUser = await _repo.Register(userToCreate, userRegistrationDto.Password);

            return StatusCode(201);
        }
    }
}