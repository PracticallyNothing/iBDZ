using System;

namespace iBDZ.Data.ViewModels
{
	public class SeatSearchResultModel
	{
		public string TrainId { get; set; }

		public string Delay { get; set; }
		public string Date { get; set; }
		public string TimeOfDeparture { get; set; }
		public string TimeOfArrival { get; set; }

		public string CarId { get; set; }

		public string Type { get; set; }
		public string Class { get; set; }

		public int CoupeNumber { get; set; }

		public int FreeSeats { get; set; }
		public int MaxSeats { get; set; }
	}
}
