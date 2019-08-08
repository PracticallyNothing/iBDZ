using System;
using System.Collections.Generic;
using System.Text;

namespace iBDZ.Data.ViewModels
{
    public class ReservationInfoModel
    {
		public string TrainId { get; set; }

		public string CarId { get; set; }
		public TrainCarType Type { get; set; }
		public TrainCarClass Class { get; set; }

		public int Coupe { get; set; }
		public List<Tuple<string, int>> FreeSeats { get; set; }

		public decimal BasePrice { get; set; }
    }
}
