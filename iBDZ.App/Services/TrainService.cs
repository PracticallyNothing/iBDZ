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
		private readonly ApplicationDbContext db;
		private readonly IRouteService routeService;

		public TrainService(ApplicationDbContext db, IRouteService routeService)
		{
			this.db = db;
			this.routeService = routeService;
		}

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

		public ShortTrainInfoModel GetShortTrainInfo(Train t)
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

		public Tuple<List<ShortTrainInfoModel>, string, string> GetTimetable(string start, string end)
		{
			List<ShortTrainInfoModel> res = new List<ShortTrainInfoModel>();
			List<Train> trains = db.Trains.Include(x => x.Route).OrderBy(x => x.Route.ToString()).ThenBy(x => x.TimeOfDeparture).ToList();

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

		public TrainInfoModel GetTrainInfoFromId(string trainId)
		{
			Train t = db.Trains
				.Include(x => x.Route)
				.Include(x => x.Cars)
				.First(x => x.Id == trainId);

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

		// WARNING: Fragile. Do not give unexpected input.
		private DateTime DecodeDate(string dateString) {
			Regex regex = new Regex(@"^(\d{2}):(\d{2})\s*([0-3]\d)\.([01]\d)\.(\d{4,})$");
			Match match = regex.Match(dateString);

			int hours = int.Parse(match.Groups[1].Value);
			int minutes = int.Parse(match.Groups[2].Value);

			int year = int.Parse(match.Groups[5].Value);
			int month = int.Parse(match.Groups[4].Value);
			int date = int.Parse(match.Groups[3].Value);

			return new DateTime(year, month, date, hours, minutes, 0);
		}

		public void EditTrain(string json)
		{
			JObject obj = JObject.Parse(json);

			Train t = db.Trains.Find(obj.GetValue("Id").Value<string>());
			t.RouteId = obj.GetValue("RouteId").Value<string>();
			t.TimeOfDeparture = DecodeDate(obj.GetValue("TimeOfDeparture").Value<string>());
			t.TimeOfArrival = DecodeDate(obj.GetValue("TimeOfArrival").Value<string>());
			t.Type = (TrainType) Enum.Parse(typeof(TrainType), obj.GetValue("Type").Value<string>());

			db.Trains.Update(t);
			db.SaveChanges();
		}

		public void DeleteTrain(string id)
		{
			db.Trains.Remove(db.Trains.Find(id));
			db.SaveChanges();
		}

		public string GenerateNewTrain()
		{
			Train t = TrainSeeder.GenTrain(db);
			db.Trains.Add(t);
			db.SaveChanges();
			return t.Id;
		}
	}
}
