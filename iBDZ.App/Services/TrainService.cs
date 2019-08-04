using iBDZ.App.Data;
using iBDZ.Data;
using iBDZ.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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
	}
}
