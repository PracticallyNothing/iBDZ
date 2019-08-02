using System;
using System.Collections.Generic;

namespace iBDZ.Data
{
	public enum TrainType {
		LightRail,
		HighspeedRail
	};
	
    public class Train
    {
		public string Id { get; set; }
		public List<TrainCar> Cars { get; set; }
		public TrainType Type { get; set; }
		public Route Route { get; set; }
		public DateTime TimeOfDeparture { get; set; }
		public DateTime TimeOfArrival { get; set; }
		public DateTime Delay { get; set; }
	}
}
