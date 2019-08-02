﻿using System.Collections.Generic;

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
		public string Id { get; set; }
		public TrainCarClass Class { get; set; }
		public TrainCarType Type { get; set; }
		public List<Seat> Seats { get; set; }
	}
}