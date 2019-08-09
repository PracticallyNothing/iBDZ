using System;
using System.Collections.Generic;
using System.Text;

namespace iBDZ.Data.ViewModels
{
	public class ShortReceiptModel
	{
		public string Id { get; set; }
		public string Route { get; set; }
		public DateTime TimeOfPurchase { get; set; }
		public decimal PriceLevs { get; set; }
	}
}
