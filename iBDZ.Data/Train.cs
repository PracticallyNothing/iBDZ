using System;
using System.Collections.Generic;

namespace iBDZ.Data
{
	public enum TrainType {
		LightRail,
		HighspeedRail,
		NightTrain
	};
	
    public class Train
    {
		public Train()
		{
			Cars = new List<TrainCar>();
			Delay = new DateTime();
		}

		public string Id { get; set; }

		public List<TrainCar> Cars { get; set; }

		public TrainType Type { get; set; }

		public string RouteId { get; set; }
		public Route Route { get; set; }

		public DateTime TimeOfDeparture { get; set; }
		public DateTime TimeOfArrival { get; set; }
		public DateTime Delay { get; set; }
	}
}
