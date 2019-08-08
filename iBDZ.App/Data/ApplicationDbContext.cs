using iBDZ.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace iBDZ.App.Data
{
	public class ApplicationDbContext : IdentityDbContext<User>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Train>().HasOne(x => x.Route).WithMany(x => x.Trains).HasForeignKey(x => x.RouteId);
			builder.Entity<TrainCar>().HasOne(x => x.Train).WithMany(x => x.Cars).HasForeignKey(x => x.TrainId);
			builder.Entity<Seat>().HasOne(x => x.Car).WithMany(x => x.Seats).HasForeignKey(x => x.CarId);

			base.OnModelCreating(builder);
		}

		public DbSet<Train> Trains { get; set; }
		public DbSet<TrainCar> TrainCars { get; set; }
		public DbSet<Route> Routes { get; set; }
		public DbSet<Seat> Seats { get; set; }
		public DbSet<Purchase> Purchases { get; set; }
		public DbSet<Receipt> Receipts { get; set; }
	}
}
