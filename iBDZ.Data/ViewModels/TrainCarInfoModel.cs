using System.Collections.Generic;

namespace iBDZ.Data.ViewModels
{
	public class TrainCarInfoModel
	{
		public string Id { get; set; }

		public TrainCarType Type { get; set; }
		public TrainCarClass Class { get; set; }

		public int FreeSeats { get; set; }
		public int MaxSeats { get; set; }

		public List<SeatInfoModel> SeatInfo { get; set; }
	}
}