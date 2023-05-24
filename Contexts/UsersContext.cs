using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RentACarAPI.Contexts
{
    public class UsersContext : IdentityUserContext<IdentityUser>
    {

        static readonly string connectionString = "Server=localhost; Port=3306; User ID=root; Password=root; Database=rentacar";

        public UsersContext (DbContextOptions<UsersContext> options) : base(options) 
        { 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
