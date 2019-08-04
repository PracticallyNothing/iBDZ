namespace iBDZ.Data.ViewModels
{
	public class RouteInfoModel
	{
		public string RouteId { get; set; }
		
		public string StartStation { get; set; }

		public string MiddleStation { get; set; }

		public string EndStation { get; set; }
		
		public override string ToString()
		{
			return StartStation + " - " + (MiddleStation == null ? "" : MiddleStation + " - ") + EndStation;
		}
	}
}