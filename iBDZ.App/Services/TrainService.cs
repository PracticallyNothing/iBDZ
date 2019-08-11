using iBDZ.App.Data;
using iBDZ.App.Data.Seeders;
using iBDZ.Data;
using iBDZ.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace iBDZ.Services
{
	public class TrainService : ITrainService
	{
		private readonly iBDZDbContext db;
		private readonly IRouteService routeService;

		public TrainService(iBDZDbContext db, IRouteService routeService)
		{
			this.db = db;
			this.routeService = routeService;
		}

		// Not called outside of class, no protection needed.
		private int CountSeatsOnTrain(Train t)
		{
			return db.TrainCars
				.Where(x => x.TrainId == t.Id)
				.Sum(
					x => db.Seats
					.Where(y => y.CarId == x.Id)
					.Count()
				);
		}

		// Not called outside of class, no protection needed.
		private int CountFreeSeatsOnTrain(Train t)
		{
			return db.TrainCars
				.Where(x => x.TrainId == t.Id)
				.Sum(
					x => db.Seats.Include(y => y.Reserver)
					.Where(y => y.CarId == x.Id && y.Reserver == null)
					.Count()
				);
		}

		// Not called outside of class, no protection needed.
		private ShortTrainInfoModel GetShortTrainInfo(Train t)
		{
			return new ShortTrainInfoModel
			{
				TrainId = t.Id,
				Route = routeService.GetShortRouteStringFromId(t.RouteId),
				TimeOfDeparture = t.TimeOfDeparture,
				TimeOfArrival = t.TimeOfArrival,
				Delay = t.Delay,
				FreeSeats = CountFreeSeatsOnTrain(t),
				MaxSeats = CountSeatsOnTrain(t),
			};
		}

		// Bad input means the user won't see anything, but that isn't a security risk.
		public Tuple<List<ShortTrainInfoModel>, string, string> GetTimetable(string start, string end)
		{
			List<ShortTrainInfoModel> res = new List<ShortTrainInfoModel>();
			List<Train> trains =
				db.Trains
					.Include(x => x.Route)
					.Where(x => x.TimeOfArrival > DateTime.Now)
					.OrderBy(x => x.Route.ToString())
					.ThenBy(x => x.TimeOfDeparture)
					.ToList();

			if (start != "" && start != null)
				trains = trains.Where(x => x.Route.StartStation == start).ToList();

			if (end != "" && end != null)
				trains = trains.Where(x => x.Route.EndStation == end).ToList();

			foreach (var t in trains)
			{
				res.Add(GetShortTrainInfo(t));
			}

			return Tuple.Create(res, start, end);
		}

		// Protected from bad input.
		public TrainInfoModel GetTrainInfoFromId(string trainId)
		{
			Train t = db.Trains
				.Include(x => x.Route)
				.Include(x => x.Cars)
				.FirstOrDefault(x => x.Id == trainId);

			// Returns empty object on bad Id.
			if (t == null)
				return new TrainInfoModel();

			// Errors beyond here are a result of a corrupted Db, not bad input.

			TrainInfoModel result = new TrainInfoModel
			{
				Id = trainId,
				TimeOfArrival = t.TimeOfArrival,
				TimeOfDeparture = t.TimeOfDeparture,
				Delay = t.Delay,
				Type = t.Type,
				FreeSeats = CountFreeSeatsOnTrain(t),
				MaxSeats = CountSeatsOnTrain(t),
			};

			result.RouteInfo = new RouteInfoModel
			{
				RouteId = t.Route.Id,
				StartStation = t.Route.StartStation,
				MiddleStation = t.Route.MiddleStation,
				EndStation = t.Route.EndStation,
			};

			result.TrainCars = new List<TrainCarInfoModel>();

			foreach (TrainCar c in t.Cars)
			{
				TrainCarInfoModel car = new TrainCarInfoModel
				{
					Id = c.Id,
					Class = c.Class,
					Type = c.Type,
					SeatInfo = new List<SeatInfoModel>(),
				};

				foreach (Seat s in db.Seats.Include(x => x.Reserver).Where(x => x.CarId == c.Id))
				{
					car.SeatInfo.Add(new SeatInfoModel
					{
						Id = s.Id,
						Number = s.SeatNumber,
						Coupe = s.Coupe,
						Taken = s.Reserver != null,
					});
				}
				car.FreeSeats = car.SeatInfo.Count(x => !x.Taken);
				car.MaxSeats = car.SeatInfo.Count;

				result.TrainCars.Add(car);
			}

			return result;
		}

		// Throws when date is not in correct format.
		private DateTime DecodeDate(string dateString)
		{
			Regex regex = new Regex(@"^(\d{2}):(\d{2})\s*([0-3]\d)\.([01]\d)\.(\d{4,})$");
			Match match = regex.Match(dateString);
			if (!match.Success)
				throw new Exception("Date is in wrong format.");

			int hours = int.Parse(match.Groups[1].Value);
			int minutes = int.Parse(match.Groups[2].Value);

			int year = int.Parse(match.Groups[5].Value);
			int month = int.Parse(match.Groups[4].Value);
			int date = int.Parse(match.Groups[3].Value);

			return new DateTime(year, month, date, hours, minutes, 0);
		}

		// Protected from bad input.
		public string EditTrain(string json)
		{
			try
			{
				JObject obj = JObject.Parse(json);

				// Null if train doesn't exist, access will throw.
				Train t = db.Trains.Find(obj.GetValue("Id").Value<string>());
				// Throws if RouteId is not valid.
				t.Route = db.Routes.First(x => x.Id == obj.GetValue("RouteId").Value<string>());
				// Throws if Date has incorrect format.
				t.TimeOfDeparture = DecodeDate(obj.GetValue("TimeOfDeparture").Value<string>());
				// Ditto.
				t.TimeOfArrival = DecodeDate(obj.GetValue("TimeOfArrival").Value<string>());
				// Throws if type is invalid.
				t.Type = (TrainType)Enum.Parse(typeof(TrainType), obj.GetValue("Type").Value<string>());

				// Throws if dates are in an incorrect order.
				if (t.TimeOfArrival <= t.TimeOfDeparture)
					throw new Exception("Train arrives before it departs.");

				// Can't throw, but doesn't need to, since we can't get corrupted data here.
				db.Trains.Update(t);
				db.SaveChanges();
				return t.Id;
			}
			catch
			{
				// Ignore bad input and don't react.
				// Any bad JSON comes from bad actors, not users.
				return "";
			}

		}

		// Protected from bad input.
		public void DeleteTrain(string id)
		{
			Train t = db.Trains.Single(x => x.Id == id);
			db.Trains.Remove(t);
			db.SaveChanges();
		}

		// Doesn't need protection, no input is passed.
		public string GenerateNewTrain()
		{
			Train t = TrainSeeder.GenTrain(db);
			db.Trains.Add(t);
			db.SaveChanges();
			return t.Id;
		}
	}
}
