using Microsoft.EntityFrameworkCore;

namespace dotnet_webapi_jwt_auth.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {}

        public DbSet<User> Users { get; set; }
    }

}