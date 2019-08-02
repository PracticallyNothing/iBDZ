namespace iBDZ.Data
{
	public class Seat
	{
		public string Id { get; set; }

		public int SeatNumber { get; set; }

		public int Coupe { get; set; }

		public TrainCar Car { get; set; }

		public User Reserver { get; set; } = null;
	}
}