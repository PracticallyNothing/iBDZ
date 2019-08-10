using iBDZ.App.Data;
using iBDZ.Data;
using iBDZ.Data.ViewModels;
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

		public List<ShortRouteModel> GetAllRoutes()
		{
			List<ShortRouteModel> result = new List<ShortRouteModel>(db.Routes.Count());
			
			foreach(var r in db.Routes)
			{
				result.Add(new ShortRouteModel
				{
					Id = r.Id,
					Route = r.ToString()
				});
			}
			return result.OrderBy(x => x.Route).ToList();
		}
	}
}
