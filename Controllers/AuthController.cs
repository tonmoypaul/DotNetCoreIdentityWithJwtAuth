using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using dotnet_webapi_jwt_auth.Dtos;
using dotnet_webapi_jwt_auth.Helpers;
using dotnet_webapi_jwt_auth.Models;
using dotnet_webapi_jwt_auth.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_webapi_jwt_auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        private readonly AppSettings _appSettings;

        public AuthController(IAuthRepository repo, IOptions<AppSettings> appSettings)
        {
            this._repo = repo;
            this._appSettings = appSettings.Value;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserLoginDto userLoginDto)
        {
            var user = await _repo.Login(userLoginDto.Username.ToLower(), userLoginDto.Password);

            if (user == null)
                return Unauthorized();
            
            // generate a jwt auth token
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);
            var tokenString = tokenhandler.WriteToken(token);

            return Ok(new {tokenString});
        }
    }
}