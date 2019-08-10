using System;
using System.Collections.Generic;
using System.Text;

namespace iBDZ.Data.ViewModels
{
    public class TrainInfoModel
    {
		public TrainInfoModel() {
			Id = "No such train";
			RouteInfo = new RouteInfoModel()
			{
				RouteId = "",
				StartStation = "Nowhere",
				MiddleStation = "Nowhere",
				EndStation = "Nowhere"
			};
			TimeOfDeparture = DateTime.Now;
			TimeOfArrival = DateTime.Now.AddMinutes(1);
			Delay = new DateTime();

			FreeSeats = 0;
			MaxSeats = 0;

			TrainCars = new List<TrainCarInfoModel>();
		}

		public string Id { get; set; }

		public TrainType Type { get; set; }

		public RouteInfoModel RouteInfo { get; set; }

		public DateTime TimeOfDeparture { get; set; }
		public DateTime TimeOfArrival { get; set; }
		public DateTime Delay { get; set; }

		public int FreeSeats { get; set; }
		public int MaxSeats { get; set; }

		public List<TrainCarInfoModel> TrainCars { get; set; }
	}
}
