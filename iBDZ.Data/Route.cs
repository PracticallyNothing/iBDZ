﻿using System;
using System.Collections.Generic;

namespace iBDZ.Data
{
	public class Route
	{
		public string Id { get; set; }
		public string StartStation { get; set; }
		public string MiddleStation { get; set; }
		public string EndStation { get; set; }

		public override string ToString()
		{
			return StartStation + " - " + (MiddleStation == null ? "" : MiddleStation + " - ") + EndStation;
		}

		public List<Train> Trains { get; set; }
	}
}
