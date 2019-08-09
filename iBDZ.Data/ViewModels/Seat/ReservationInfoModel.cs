using System;
using System.Collections.Generic;
using System.Text;

namespace iBDZ.Data.ViewModels
{
	public class ReservationSeatInfo {
		public string Id { get; set; }
		public int SeatNumber { get; set; }
		public bool Taken { get; set; }
	}
	
    public class ReservationInfoModel
    {
		public string TrainId { get; set; }

		public string CarId { get; set; }
		public TrainCarType Type { get; set; }
		public TrainCarClass Class { get; set; }

		public int Coupe { get; set; }
		public List<ReservationSeatInfo> SeatInfo { get; set; }
		public decimal BasePrice { get; set; }

		public List<ReservationSeatInfo> FreeSeats { get; set; }
	}
}
