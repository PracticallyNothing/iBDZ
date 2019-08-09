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

		public List<Purchase> Purchases { get; set; }
	}
}
