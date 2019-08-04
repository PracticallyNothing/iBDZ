using System.Collections.Generic;

namespace iBDZ.Data
{
	public enum TrainCarClass
	{
		Business, First, Second
	}

	public enum TrainCarType
	{
		Compartments, Beds
	}

	public class TrainCar
	{
		public TrainCar()
		{
			Seats = new List<Seat>();
		}

		public string Id { get; set; }
		public TrainCarClass Class { get; set; }
		public TrainCarType Type { get; set; }

		public string TrainId { get; set; }
		public Train Train { get; set; }

		public List<Seat> Seats { get; set; }
	}
}