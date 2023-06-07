using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Models;
using System.Reflection.Emit;

namespace RentACarAPI.Contexts
{
    public class DataContext : IdentityDbContext<Owner>
    {

        static readonly string connectionString = "Server=localhost; Port=3306; User ID=root; Password=root; Database=rentacar";

        public DbSet<Car> Cars { get; set; }
        public DbSet<Position> Positions { get; set; }

        public DbSet<CarType> CarTypes { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CarType>()
                .HasData(Enum.GetValues(typeof(CarTypeEnum))
                    .Cast<CarTypeEnum>()
                    .Select(e => new CarType
                    {
                        Id = (short)e + 1,
                        Type = e.ToString()
                    })
            );

            builder.Entity<Car>()
                .HasOne(c => c.Position)
                .WithOne()
                .HasForeignKey<Car>(c => c.PositionId);

            builder.Entity<Car>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.Cars)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
