using iBDZ.App.Data;

namespace iBDZ.Services
{
	public class RouteService : IRouteService
	{
		private readonly ApplicationDbContext db;

		public RouteService(ApplicationDbContext db)
		{
			this.db = db;
		}

		public string GetShortRouteStringFromId(string routeId)
		{
			return db.Routes.Find(routeId).ToString();
		}
	}
}
