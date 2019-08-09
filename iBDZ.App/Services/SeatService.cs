using iBDZ.App.Data;
using iBDZ.Data;
using iBDZ.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace iBDZ.Services
{
	public class SeatService : ISeatService
	{
		private readonly ApplicationDbContext db;

		public SeatService(ApplicationDbContext db)
		{
			this.db = db;
		}

		private int GetCoupeSeats(TrainCar c)
		{
			switch (c.Type)
			{
				case TrainCarType.Beds:
					return 3;
				case TrainCarType.Compartments:
					switch (c.Class)
					{
						case TrainCarClass.Business:
						case TrainCarClass.First: return 6;
						case TrainCarClass.Second: return 8;
					};
					break;
			}

			return int.MinValue;
		}

		public List<SeatSearchResultModel> FindSeats(string jsonParams)
		{
			JObject o = JObject.Parse(jsonParams);

			string RouteId = o["RouteId"].Value<string>();
			TrainCarType TrainCarType = Enum.Parse<TrainCarType>(o["Type"].Value<string>());
			string ClassString = o["Class"].Value<string>().Trim();
			List<TrainCar> filtered;

			if (ClassString == "Any")
			{
				filtered = db.TrainCars
					.Include(x => x.Seats).ThenInclude(x => x.Reserver)
					.Include(x => x.Train).ThenInclude(x => x.Route)
					.Where(x => x.Train.RouteId == RouteId
							 && x.Type == TrainCarType)
					.ToList();
			}
			else
			{
				filtered = db.TrainCars
					.Include(x => x.Seats).ThenInclude(x => x.Reserver)
					.Include(x => x.Train).ThenInclude(x => x.Route)
					.Where(x => x.Train.RouteId == RouteId
							 && x.Type == TrainCarType
							 && x.Class == Enum.Parse<TrainCarClass>(ClassString))
					.ToList();
			}

			List<SeatSearchResultModel> res = new List<SeatSearchResultModel>();

			foreach (var c in filtered)
			{
				var coupeFreeSeats = c.Seats
					.Where(x => x.Reserver == null)
					.OrderBy(x => x.Coupe)
					.GroupBy(x => x.Coupe)
					.Select(x => new { Coupe = x.Key, Count = x.Count() })
					.Where(x => x.Count >= o["NumSeats"].Value<int>())
					.ToList();

				foreach (var cc in coupeFreeSeats)
				{
					res.Add(new SeatSearchResultModel()
					{
						TrainId = c.Train.Id,
						CarId = c.Id.Substring(0, 4),
						Type = c.Type.ToString(),
						Class = c.Class.ToString(),
						CoupeNumber = cc.Coupe,
						FreeSeats = cc.Count,
						MaxSeats = GetCoupeSeats(c)
					});
				}
			}

			return res;
		}

		public string ReserveSeat(ClaimsPrincipal user, string jsonString)
		{
			JObject json = JObject.Parse(jsonString);
			var jsonSeats = (JArray)json["seats"];
			List<Seat> seats = new List<Seat>(jsonSeats.Count);

			foreach (var j in jsonSeats)
			{
				seats.Add(
					db.Seats.Include(x => x.Car).Where(x => x.Id == j.Value<string>()).First()
				);
			}

			Receipt receipt = new Receipt()
			{
				User = db.Users.Where(x => x.UserName == user.Identity.Name).First(),
				TimeOfPurchase = DateTime.Now,
				PriceLevs = GetBasePrice(seats[0].Car.Type, seats[0].Car.Class) * jsonSeats.Count,
			};

			foreach (Seat s in seats)
			{
				Purchase p = new Purchase() { Seat = s, Receipt = receipt };
				p.Seat.Reserver = receipt.User;

				receipt.Purchases.Add(p);
				db.Purchases.Add(p);
			}

			db.Receipts.Add(receipt);
			db.SaveChanges();
			return receipt.Id;
		}

		private static decimal GetBasePrice(TrainCarType Type, TrainCarClass Class)
		{
			switch (Type)
			{
				case TrainCarType.Beds:
					return 45.99m;
				case TrainCarType.Compartments:
					switch (Class)
					{
						case TrainCarClass.Business: return 34.99m;
						case TrainCarClass.First: return 29.99m;
						case TrainCarClass.Second: return 20.99m;
					}
					break;
			}

			return -1m;
		}

		public ReservationInfoModel GetReservationInfo(string car, string coupe)
		{
			List<Seat> seats =
				db.Seats
				.Include(x => x.Car).ThenInclude(x => x.Train)
				.Include(x => x.Reserver)
				.Where(x => x.Car.Id.StartsWith(car) && x.Coupe == int.Parse(coupe))
				.ToList();

			ReservationInfoModel res = new ReservationInfoModel()
			{
				CarId = seats[0].Car.Id,
				TrainId = seats[0].Car.Train.Id,
				Class = seats[0].Car.Class,
				Type = seats[0].Car.Type,
				Coupe = int.Parse(coupe),
				BasePrice = GetBasePrice(seats[0].Car.Type, seats[0].Car.Class),
				SeatInfo = seats.Select(x => new ReservationSeatInfo
				{
					Id = x.Id,
					SeatNumber = x.SeatNumber + x.Coupe * 10,
					Taken = x.Reserver != null,
				}).ToList(),
				FreeSeats = seats.Where(x => x.Reserver == null).Select(x => new ReservationSeatInfo
				{
					Id = x.Id,
					SeatNumber = x.SeatNumber + x.Coupe * 10,
					Taken = true,
				}).ToList(),
			};

			return res;
		}
	}
}
