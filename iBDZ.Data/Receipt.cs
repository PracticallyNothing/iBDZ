using System;
using System.Collections.Generic;
using System.Text;

namespace iBDZ.Data
{
	public class Receipt
	{
		public Receipt()
		{
			Purchases = new List<Purchase>();
		}

		public string Id { get; set; }

		public DateTime TimeOfPurchase { get; set; }

		public decimal PriceLevs { get; set; }

		public User User { get; set; }

		public string TrainId { get; set; }
		public string Route { get; set; }
		public DateTime TimeOfDeparture { get; set; }
		public DateTime TimeOfArrival { get; set; }

		/// <summary>
		/// Can the purchase still be refunded?
		/// </summary>
		/// A purchase is refundable if:
		/// - a week from it being made has yet to pass 
		///     and 
		/// - there are more than 20 minutes until the train departs.
		public bool IsRefundable => DateTime.Now < TimeOfDeparture.AddMinutes(20)
							     && DateTime.Now < TimeOfPurchase.AddDays(7);

		public List<Purchase> Purchases { get; set; }
	}
}
