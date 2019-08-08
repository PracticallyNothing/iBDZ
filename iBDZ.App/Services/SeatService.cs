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
					.Include(x => x.Seats)
					.Include(x => x.Train).ThenInclude(x => x.Route)
					.Where(x => x.Train.RouteId == RouteId
							 && x.Type == TrainCarType)
					.ToList();
			}
			else
			{
				filtered = db.TrainCars
					.Include(x => x.Seats)
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
			Receipt receipt = new Receipt();

			JObject json = JObject.Parse(jsonString);
			List<string> seats = json["seats"].Value<List<string>>();
			
			foreach(string SeatId in seats)
			{
				Purchase p = new Purchase()
				{
					TimeOfPurchase = DateTime.Now,
					PriceLevs = json["priceLevs"].Value<decimal>(),
					User = db.Users.Where(x => x.UserName == user.Identity.Name).First(),
					Seat = db.Seats.Find(SeatId),
					Receipt = receipt,
				};

				p.Seat.Reserver = p.User;

				receipt.Purchases.Add(p);
				db.Purchases.Add(p);
			}

			db.Receipts.Add(receipt);
			db.SaveChanges();
			return receipt.Id;
		}
	}
}
