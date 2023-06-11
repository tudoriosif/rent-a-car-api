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

        public DbSet<Owner> Owners { get; set; }

        public DbSet<RentingEvent> RentingEvents { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            options.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CarType>()
                .HasData(Enum.GetValues(typeof(CarTypeEnum))
                    .Cast<CarTypeEnum>()
                    .Select(e => new CarType
                    {
                        Id = (short)e,
                        Type = e.ToString()
                    })
            );

            builder.Entity<Car>()
                .HasOne(c => c.Position)
                .WithOne()
                .HasForeignKey<Car>(c => c.PositionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Car>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.Cars)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Owner>()
                .HasMany(o => o.RentingEvents)
                .WithOne(re => re.Owner)
                .HasForeignKey(re => re.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RentingEvent>()
                .HasOne(re => re.Car)
                .WithMany(c => c.RentingEvents)
                .HasForeignKey(re => re.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
