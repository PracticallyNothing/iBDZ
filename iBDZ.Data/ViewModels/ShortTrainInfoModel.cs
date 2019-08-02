using System;
using System.Collections.Generic;
using System.Text;

namespace iBDZ.Data.ViewModels
{
	public class ShortTrainInfoModel
	{
		public string TrainId { get; set; }

		public string Route { get; set; }

		public DateTime TimeOfDeparture { get; set; }

		public DateTime TimeOfArrival { get; set; }

		public DateTime Delay { get; set; }

		public int FreeSpaces { get; set; }
	}
}
