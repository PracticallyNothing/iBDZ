using System;
using System.Collections.Generic;
using System.Text;

namespace iBDZ.Data.ViewModels
{
    public class ReceiptModel
    {
		public string Id { get; set; }

		public string TrainId { get; set; }
		public string Route { get; set; }
		public DateTime TimeOfDeparture { get; set; }
		public DateTime TimeOfArrival { get; set; }

		public string CarId { get; set; }
		public TrainCarType Type { get; set; }
		public TrainCarClass Class { get; set; }

		public int Coupe { get; set; }
		public List<int> ReservedSeatNumbers { get; set; }

		public DateTime TimeOfPurchase { get; set; }
		public decimal PriceLevs { get; set; }
    }
}
