using iBDZ.App.Data;
using iBDZ.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace iBDZ.App.Helpers
{
	public class TrainManager
	{
		private Thread thread;

		private IConfiguration configuration;
		public bool IsRunning { get; set; } = false;

		public TrainManager(IConfiguration configuration)
		{
			thread = new Thread(new ThreadStart(RepurposeTrains));
			this.configuration = configuration;
		}

		public void Start()
		{
			IsRunning = true;
			thread.Start();
		}

		public void Stop()
		{
			IsRunning = false;
			thread.Join();
		}

		private void RepurposeTrains()
		{
			while (IsRunning)
			{
				Random r = new Random();
				List<Train> trains;

				var opts = new DbContextOptionsBuilder<iBDZDbContext>();
				opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

				using (iBDZDbContext db = new iBDZDbContext(opts.Options))
				{
					trains = db.Trains
						.Include(x => x.Route)
						.Include(x => x.Cars)
							.ThenInclude(x => x.Seats)
							.ThenInclude(x => x.Reserver)
						.Where(x => x.TimeOfArrival <= DateTime.Now)
						.ToList();

					// NOTE: Throws if reverse version of train's route doesn't exist.
					//       This only applies to manual Db seeding, the automatic system
					//       always adds the reverse version of the route as well.
					trains.ForEach(x =>
					{
						var availableRoutes = db.Routes.Where(y => y.StartStation == x.Route.EndStation).ToList();
						x.TimeOfDeparture = DateTime.Now.AddHours(r.Next(24, 24 * 7));
						x.TimeOfDeparture += new TimeSpan(0, 30 * r.Next(0, 1), 0);
						x.TimeOfArrival = x.TimeOfDeparture.AddMinutes(r.Next(7, 16) * 30);
						x.Route = availableRoutes.Skip(r.Next(0, availableRoutes.Count - 1)).First();
						x.Cars.ForEach(y => y.Seats.ForEach(z => z.Reserver = null));
					});

					db.Trains.UpdateRange(trains);
					db.SaveChanges();
				}

				Thread.Sleep(new TimeSpan(0, 30, 0));
			}
		}
	}
}
