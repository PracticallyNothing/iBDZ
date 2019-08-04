using System;

namespace iBDZ.Data.ViewModels
{
	public class ShortTrainInfoModel
	{
		public string TrainId { get; set; }

		public string Route { get; set; }

		public DateTime TimeOfDeparture { get; set; }

		public DateTime TimeOfArrival { get; set; }

		public DateTime Delay { get; set; } = new DateTime();

		public int FreeSeats { get; set; }

		public int MaxSeats { get; set; }
	}
}
