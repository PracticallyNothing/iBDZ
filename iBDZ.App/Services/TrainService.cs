using iBDZ.App.Data;
using iBDZ.Data;
using iBDZ.Data.ViewModels;
using System;
using System.Collections.Generic;

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

		public int CountFreeSpacesOnTrain(Train t)
		{
			int freeSpaces = 0;

			foreach (var car in t.Cars)
			{
				foreach (var seat in car.Seats)
				{
					if (seat.Reserver == null)
					{
						freeSpaces++;
					}
				}
			}

			return freeSpaces;
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
				FreeSpaces = CountFreeSpacesOnTrain(t),
			};
		}

		public List<ShortTrainInfoModel> GetTimetable()
		{
			List<ShortTrainInfoModel> res = new List<ShortTrainInfoModel>();

			foreach (var t in db.Trains)
			{
				res.Add(GetShortTrainInfo(t));
			}

			return res;
		}

		public TrainInfoModel GetTrainFromId(string trainId)
		{
			return new TrainInfoModel { };
		}
	}
}
