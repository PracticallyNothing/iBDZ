using System;

namespace iBDZ.Data
{
	public class Purchase
    {
		public string Id { get; set; }

		public Seat Seat { get; set; }

		public Receipt Receipt { get; set; }
    }
}
