using iBDZ.App.Data;
using iBDZ.Data;
using System.Collections.Generic;
using System.Linq;

namespace iBDZ.Services
{
	public class RouteService : IRouteService
	{
		private readonly ApplicationDbContext db;

		public RouteService(ApplicationDbContext db)
		{
			this.db = db;
		}

		public Route GetRouteFromId(string id)
		{
			return db.Routes.Find(id);
		}

		public string GetShortRouteStringFromId(string routeId)
		{
			return db.Routes.Find(routeId).ToString();
		}

		public List<string> GetAllStartingStations()
		{
			return db.Routes.Select(x => x.StartStation).Distinct().OrderBy(x => x).ToList();
		}

		public List<string> GetAllEndStations()
		{
			return db.Routes.Select(x => x.EndStation).Distinct().OrderBy(x => x).ToList();
		}
	}
}
