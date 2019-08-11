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
		private readonly iBDZDbContext db;

		public SeatService(iBDZDbContext db)
		{
			this.db = db;
		}

		// Not called outside of class, no protection needed.
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

		// Protected from bad input.
		public List<SeatSearchResultModel> FindSeats(string jsonParams)
		{
			try
			{
				JObject o = JObject.Parse(jsonParams);

				// Throws if "RouteId" is not in JSON (throws later for other errors).
				string RouteId = o["RouteId"].Value<string>();
				// Throws if "Type" is not allowed or not in JSON.
				TrainCarType TrainCarType = Enum.Parse<TrainCarType>(o["Type"].Value<string>());
				// Throws if "Class" is not in JSON.
				string ClassString = o["Class"].Value<string>().Trim();

				List<TrainCar> filtered;

				if (ClassString == "Any" || TrainCarType == TrainCarType.Beds)
				{
					filtered = db.TrainCars
						.Include(x => x.Seats).ThenInclude(x => x.Reserver)
						.Include(x => x.Train).ThenInclude(x => x.Route)
						.Where(x => x.Train.Route.Id == RouteId
						         && DateTime.Now < x.Train.TimeOfDeparture.AddMinutes(5)
								 && x.Type == TrainCarType)
						.ToList();
				}
				else
				{
					// Throws if "Class" is not allowed.
					filtered = db.TrainCars
						.Include(x => x.Seats).ThenInclude(x => x.Reserver)
						.Include(x => x.Train).ThenInclude(x => x.Route)
						.Where(x => x.Train.RouteId == RouteId
								 && DateTime.Now < x.Train.TimeOfDeparture.AddMinutes(5)
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
			catch
			{
				return new List<SeatSearchResultModel>();
			}
		}

		public string ReserveSeat(ClaimsPrincipal user, string jsonString)
		{
			try
			{
				JObject json = JObject.Parse(jsonString);
				var jsonSeats = (JArray)json["seats"];
				List<Seat> seats = new List<Seat>(jsonSeats.Distinct().Count());

				if (jsonSeats.Count() == 0 || jsonSeats.Distinct().Count() > 8)
				{
					throw new Exception("Too many or 0 seats requested.");
				}

				foreach (var j in jsonSeats.Distinct())
				{
					seats.Add(
						db.Seats
							.Include(x => x.Reserver)
							.Include(x => x.Car)
								.ThenInclude(x => x.Train)
								.ThenInclude(x => x.Route)
							.First(x => x.Car.Id == json["carId"].Value<string>() 
								     && x.Id == j.Value<string>() 
								     && x.Reserver == null)
					);
				}

				if (seats.Count > GetCoupeSeats(seats[0].Car))
				{
					throw new Exception("Too many seats requested.");
				}

				if (seats.Any(x => x.Car.Id != json["carId"].Value<string>()))
				{
					throw new Exception("Seats are not from requested train car.");
				}

				if (!seats.All(x => x.Coupe == seats[0].Coupe))
				{
					throw new Exception("Seats are not from same coupe.");
				}

				Receipt receipt = new Receipt()
				{
					User = db.Users.Where(x => x.UserName == user.Identity.Name).First(),
					TimeOfPurchase = DateTime.Now,
					PriceLevs = GetBasePrice(seats[0].Car.Type, seats[0].Car.Class) * jsonSeats.Count,

					TrainId = seats[0].Car.Train.Id,
					Route = seats[0].Car.Train.Route.ToString(),
					TimeOfDeparture = seats[0].Car.Train.TimeOfDeparture,
					TimeOfArrival = seats[0].Car.Train.TimeOfArrival,
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
			catch
			{
				return "";
			}
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
			List<Seat> seats;
			try
			{
				seats = db.Seats
					.Include(x => x.Car).ThenInclude(x => x.Train)
					.Include(x => x.Reserver)
					.Where(x => x.Car.Id.StartsWith(car) && x.Coupe == int.Parse(coupe))
					.ToList();
			}
			catch
			{
				return new ReservationInfoModel();
			}

			if (seats.Count == 0)
			{
				return new ReservationInfoModel();
			}

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
