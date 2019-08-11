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
			AddTrains(serviceProvider);
		}

		public static void AddTrains(IServiceProvider serviceProvider)
		{
			var db = serviceProvider.GetRequiredService<iBDZDbContext>();
			if (db.Trains.Count() < NumTrains && db.Routes.Any())
			{
				for (int i = 0; i < NumTrains - db.Trains.Count(); i++)
				{
					Train t = GenTrain(db);
					db.Trains.Add(t);
				}
			}
			db.SaveChanges();
		}

		public static Train GenTrain(iBDZDbContext db)
		{
			Random r = new Random();

			DateTime today = DateTime.Today;
			DateTime timeOfDep = today.AddHours(r.NextDouble() * 7 + 4);
			DateTime timeOfArrival = timeOfDep.AddMinutes(r.NextDouble() * 120 + 125);

			List<Route> routes = db.Routes.ToList();

			Train t = new Train()
			{
				Route = routes.Skip(r.Next(0, db.Routes.Count() - 1)).First(),
				TimeOfDeparture = timeOfDep,
				TimeOfArrival = timeOfArrival,
				Type = (TrainType)Enum.GetValues(typeof(TrainType)).GetValue(r.Next(Enum.GetValues(typeof(TrainType)).Length)),
			};

			t.RouteId = t.Route.Id;

			FillTrainCars(r.Next(MinNumCars, MaxNumCars), t, db);
			return t;
		}

		private const int NumTrains = 50;
		private const int MinNumCars = 4;
		private const int MaxNumCars = 12;

		// What relative part of all train cars will be sleeping cars?
		private const double BedCarsDistrib = 0.5;
		// What relative part of all train cars will be business class?
		private const double BusinessClassDistrib = 0.5;
		// What relative part of all train cars will be first class?
		private const double FirstClassDistrib = 1;
		// What relative part of all train cars will be second class?
		private const double SecondClassDistrib = 2;

		private static void FillTrainCars(int numCars, Train train, iBDZDbContext db)
		{
			RatioDistributor rd = new RatioDistributor(SecondClassDistrib, FirstClassDistrib, BedCarsDistrib, BusinessClassDistrib);
			var carDistributions = rd.DistributeInt(numCars);

			List<TrainCar> templates = new List<TrainCar>()
			{
				new TrainCar() { Type = TrainCarType.Compartments, Class = TrainCarClass.Second },
				new TrainCar() { Type = TrainCarType.Compartments, Class = TrainCarClass.First },
				new TrainCar() { Type = TrainCarType.Beds, Class = TrainCarClass.Business },
				new TrainCar() { Type = TrainCarType.Compartments, Class = TrainCarClass.Business },
			};

			int templateIndex = 0;

			foreach (int distrib in carDistributions)
			{
				for (int i = 0; i < carDistributions[0]; i++)
				{
					TrainCar c = new TrainCar(templates[templateIndex]);
					FillSeats(c, db);
					train.Cars.Add(c);
					db.TrainCars.Add(c);
				}
				templateIndex++;
			}
		}

		private static void FillSeats(TrainCar car, iBDZDbContext db)
		{
			// Every train car is split into nine coupes...
			for (int i = 1; i <= 9; i++)
			{
				// ... however, compartment and sleeping cars have a different number of places available.
				if (car.Type == TrainCarType.Compartments)
				{
					// First and business class compartments have six seats.
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
			}
		}
	}
}
