using iBDZ.App.Data;
using iBDZ.Data;
using iBDZ.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace iBDZ.Services
{
	public class UserService : IUserService
	{
		private readonly iBDZDbContext db;

		public UserService(iBDZDbContext db)
		{
			this.db = db;
		}

		public ReceiptModel GetReceipt(string username, string rid)
		{
			Receipt receipt =
				db.Receipts
					.Include(x => x.User)
					.Include(x => x.Purchases)
						.ThenInclude(x => x.Seat)
						.ThenInclude(x => x.Car)
					.FirstOrDefault(x => x.Id == rid && x.User.UserName == username);

			if (receipt == null)
			{
				return new ReceiptModel();
			}

			Purchase p = receipt.Purchases[0];

			ReceiptModel result = new ReceiptModel()
			{
				Id = receipt.Id,

				TrainId = receipt.TrainId,
				Route = receipt.Route,
				TimeOfDeparture = receipt.TimeOfDeparture,
				TimeOfArrival = receipt.TimeOfArrival,

				CarId = p.Seat.Car.Id.Substring(0, 4),
				Type = p.Seat.Car.Type,
				Class = p.Seat.Car.Class,

				UserName = receipt.User.UserName,

				Coupe = p.Seat.Coupe,
				ReservedSeatNumbers = new List<int>(),

				TimeOfPurchase = receipt.TimeOfPurchase,
				PriceLevs = receipt.PriceLevs,

				IsRefundable = receipt.IsRefundable
			};

			foreach (Purchase purchase in db.Purchases
										 .Include(x => x.Receipt)
										 .Include(x => x.Seat)
										 .Where(x => x.Receipt == receipt)
										 .ToList())
			{
				result.ReservedSeatNumbers.Add(purchase.Seat.Coupe * 10 + purchase.Seat.SeatNumber);
			}

			return result;
		}

		public List<ShortReceiptModel> GetUserPurchasesList(string username)
		{
			List<Receipt> receipts =
				db.Receipts
					.Include(x => x.Purchases)
						.ThenInclude(x => x.Seat)
						.ThenInclude(x => x.Car)
						.ThenInclude(x => x.Train)
						.ThenInclude(x => x.Route)
					.Where(x => x.User.UserName == username)
					.ToList();

			List<ShortReceiptModel> shortReceipts = receipts
					.Select(x => new ShortReceiptModel
					{
						Id = x.Id,
						PriceLevs = x.PriceLevs,
						Route = x.Purchases[0].Seat.Car.Train.Route.ToString(),
						TimeOfPurchase = x.TimeOfPurchase,
					})
					.OrderBy(x => x.TimeOfPurchase)
					.ToList();

			return shortReceipts;
		}

		public void RefundPurchase(ClaimsPrincipal user, string id)
		{
			Receipt r = db.Receipts
				.Include(x => x.Purchases).ThenInclude(x => x.Seat)
				.Include(x => x.User)
				.FirstOrDefault(x => x.Id == id);

			// Returns on bad id.
			if (r == null)
			{
				return;
			}

			// Returns on unrefundable purchase.
			if (!r.IsRefundable)
			{
				return;
			}

			// Returns when user doesn't match.
			if (user.Identity.Name != r.User.UserName)
			{
				return;
			}

			r.Purchases.ForEach(x => { x.Seat.Reserver = null; db.Seats.Update(x.Seat); });
			db.Purchases.RemoveRange(r.Purchases);
			db.Receipts.Remove(r);
			db.SaveChanges();
		}
	}
}
