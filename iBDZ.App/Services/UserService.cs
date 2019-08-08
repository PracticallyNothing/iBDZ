using iBDZ.App.Data;
using iBDZ.Data;
using iBDZ.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace iBDZ.App.Services
{
    public class UserService : IUserService
    {
		private readonly ApplicationDbContext db;

		public UserService(ApplicationDbContext db)
		{
			this.db = db;
		}

		public ReceiptModel GetReceipt(ClaimsPrincipal user, string rid)
		{
			Receipt receipt = db.Receipts.Include(x => x.Purchases).Where(x => x.Id == rid).First();	
			
			Purchase p = 
				db.Purchases
				.Include(x => x.Seat)
					.ThenInclude(x => x.Car)
					.ThenInclude(x => x.Train)
					.ThenInclude(x => x.Route)
				.Include(x => x.User)
				.Where(x => x.User.UserName == user.Identity.Name
				         && x.Id == receipt.Purchases[0].Id)
				.First();

			ReceiptModel result = new ReceiptModel()
			{
				Id = p.Id,
				
				TrainId = p.Seat.Car.Train.Id,
				Route = p.Seat.Car.Train.Route.ToString(),
				TimeOfDeparture = p.Seat.Car.Train.TimeOfDeparture,
				TimeOfArrival = p.Seat.Car.Train.TimeOfArrival,
				
				CarId = p.Seat.Car.Id,
				Type = p.Seat.Car.Type,
				Class = p.Seat.Car.Class,
				
				Coupe = p.Seat.Coupe,
				ReservedSeatNumbers = new List<int>(),

				TimeOfPurchase = p.TimeOfPurchase,
				PriceLevs = p.PriceLevs
			};
			
			foreach(Purchase purchase in db.Purchases
			                             .Include(x => x.Receipt)
										 .Include(x => x.Seat)
										 .Where(x => x.Receipt == receipt)
										 .ToList())
			{
				result.ReservedSeatNumbers.Add(purchase.Seat.Coupe * 10 + purchase.Seat.SeatNumber);
			}

			return result;
		}

		public List<ShortPurchaseModel> GetUserPurchasesList(ClaimsPrincipal user)
		{
			List<Purchase> purchases = db.Users.Include(x => x.Purchases).Where(x => x.UserName == user.Identity.Name).Select(x => x.Purchases).First();
			List<ShortPurchaseModel> shortPurchases = new List<ShortPurchaseModel>(purchases.Count);

			foreach (var p in purchases)
			{
				shortPurchases.Add(new ShortPurchaseModel
				{
					Id = p.Id,
					Route = p.Seat.Car.Train.Route.ToString(),
					PriceLevs = p.PriceLevs,
					TimeOfPurchase = p.TimeOfPurchase
				});
			}

			return shortPurchases;
		}
	}
}
