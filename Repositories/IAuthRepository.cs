using System.Threading.Tasks;
using dotnet_webapi_jwt_auth.Models;

namespace dotnet_webapi_jwt_auth.Repositories
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}