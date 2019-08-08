using System;

namespace iBDZ.Data
{
	public class Purchase
    {
		public string Id { get; set; }

		public DateTime TimeOfPurchase { get; set; }

		public decimal PriceLevs { get; set; }

		public Seat Seat { get; set; }
		public User User { get; set; }

		public Receipt Receipt { get; set; }
    }
}
