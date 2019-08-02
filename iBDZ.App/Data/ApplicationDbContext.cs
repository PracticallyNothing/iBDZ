using iBDZ.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace iBDZ.App.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Train> Trains { get; set; }
		public DbSet<TrainCar> TrainCars { get; set; }
		public DbSet<Route> Routes { get; set; }
		public DbSet<Seat> Seats { get; set; }
	}
}
