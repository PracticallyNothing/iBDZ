using iBDZ.App.Helpers;
using iBDZ.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBDZ.App.Data.Seeders
{
	public class TrainSeeder : ISeeder
	{
		public void Seed(IServiceProvider serviceProvider)
		{
			var db = serviceProvider.GetRequiredService<ApplicationDbContext>();
			if (!db.Trains.Any() && db.Routes.Any())
			{
				for (int i = 0; i < NumTrains; i++)
				{
					Train t = GenTrain(db);
					db.Trains.Add(t);
					db.SaveChanges();
				}
			}
		}

		public static Train GenTrain(ApplicationDbContext db) {
			Random r = new Random();

			DateTime today = DateTime.Today;
			DateTime timeOfDep = today.AddHours(r.NextDouble() * 7 + 4);
			DateTime timeOfArrival = timeOfDep.AddMinutes(r.NextDouble() * 120 + 125);

			Train t = new Train()
			{
				Route = db.Routes.Skip(r.Next(0, db.Routes.Count() - 1)).First(),
				TimeOfDeparture = timeOfDep,
				TimeOfArrival = timeOfArrival,
				Type = (TrainType)Enum.GetValues(typeof(TrainType)).GetValue(r.Next(Enum.GetValues(typeof(TrainType)).Length)),
			};

			t.RouteId = t.Route.Id;

			FillTrainCars(r.Next(MinNumCars, MaxNumCars), t, db);
			return t;
		}

		private const int NumTrains = 10;
		private const int MinNumCars = 4;
		private const int MaxNumCars = 9;

		// What relative part of all train cars will be sleeping cars?
		private const double BedCarsDistrib = 0.5;
		// What relative part of all train cars will be first class?
		private const double FirstClassDistrib = 1;
		// What relative part of all train cars will be second class?
		private const double SecondClassDistrib = 2;

		private static void FillTrainCars(int numCars, Train train, ApplicationDbContext db)
		{
			RatioDistributor rd = new RatioDistributor(SecondClassDistrib, FirstClassDistrib, BedCarsDistrib);
			var carDistributions = rd.DistributeInt(numCars);

			for (int i = 0; i < carDistributions[0]; i++)
			{
				TrainCar c = new TrainCar()
				{
					Type = TrainCarType.Compartments,
					Class = TrainCarClass.Second
				};

				FillSeats(c, db);
				train.Cars.Add(c);
				db.TrainCars.Add(c);
				db.SaveChanges();
			}
			for (int i = 0; i < carDistributions[1]; i++)
			{
				TrainCar c = new TrainCar()
				{
					Type = TrainCarType.Compartments,
					Class = TrainCarClass.First
				};

				FillSeats(c, db);
				train.Cars.Add(c);
				db.TrainCars.Add(c);
				db.SaveChanges();
			}
			for (int i = 0; i < carDistributions[2]; i++)
			{
				TrainCar c = new TrainCar()
				{
					Type = TrainCarType.Beds,
					Class = TrainCarClass.Business
				};

				FillSeats(c, db);
				train.Cars.Add(c);
				db.TrainCars.Add(c);
				db.SaveChanges();
			}
		}

		private static void FillSeats(TrainCar car, ApplicationDbContext db)
		{
			// Every train car is split into nine coupes...
			for (int i = 0; i < 9; i++)
			{
				// ... however, compartment and sleeping cars have a different number of places available.
				if (car.Type == TrainCarType.Compartments)
				{
					// First class compartments have six seats.
					if (car.Class == TrainCarClass.First || car.Class == TrainCarClass.Business)
					{
						for (int j = 1; j <= 6; j++)
						{
							Seat s = new Seat { Coupe = i, SeatNumber = j, Reserver = null };
							db.Seats.Add(s);
							car.Seats.Add(s);
						}
					}
					// Second class compartments have 8 seats;
					else if (car.Class == TrainCarClass.Second)
					{
						for (int j = 1; j <= 8; j++)
						{
							Seat s = new Seat { Coupe = i, SeatNumber = j, Reserver = null };
							db.Seats.Add(s);
							car.Seats.Add(s);
						}
					}
				}
				// Sleeping cars have only three beds and the class of car doesn't dictate the quantity.
				else if (car.Type == TrainCarType.Beds)
				{
					for (int j = 1; j <= 3; j++)
					{
						Seat s = new Seat { Coupe = i, SeatNumber = j, Reserver = null };
						db.Seats.Add(s);
						car.Seats.Add(s);
					}
				}
				db.SaveChanges();
			}
		}
	}
}
