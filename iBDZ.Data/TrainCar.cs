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

		/// <summary>
		/// Copy constructor. Only copies type and class.
		/// </summary>
		/// <param name="other">TrainCar to copy</param>
		public TrainCar(TrainCar other)
		{
			Seats = new List<Seat>();
			this.Class = other.Class;
			this.Type = other.Type;
		}

		public string Id { get; set; }
		public TrainCarClass Class { get; set; }
		public TrainCarType Type { get; set; }

		public string TrainId { get; set; }
		public Train Train { get; set; }

		public List<Seat> Seats { get; set; }
	}
}