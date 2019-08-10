using iBDZ.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace iBDZ.App.Data.Seeders
{
	public class RouteSeeder : ISeeder
	{
		public void Seed(IServiceProvider serviceProvider)
		{
			var db = serviceProvider.GetRequiredService<iBDZDbContext>();
			if (!db.Routes.Any())
			{
				AddRoute(db, "Варна", "Горна Оряховица", "София");
				AddRoute(db, "Варна", "Карлово", "София");
				AddRoute(db, "Стара Загора", "Пловдив");
				AddRoute(db, "Стара Загора", "Горна Оряховица");
				AddRoute(db, "Варна", "Шумен");
			}
			db.SaveChanges();
		}

		public void AddRoute(iBDZDbContext db, string StartStation, string EndStation)
		{
			db.Routes.Add(new Route
			{
				StartStation = StartStation,
				EndStation = EndStation,
				MiddleStation = null
			});

			db.Routes.Add(new Route
			{
				StartStation = EndStation,
				EndStation = StartStation,
				MiddleStation = null
			});
		}

		public void AddRoute(iBDZDbContext db, string StartStation, string MiddleStation, string EndStation)
		{
			db.Routes.Add(new Route
			{
				StartStation = StartStation,
				EndStation = EndStation,
				MiddleStation = MiddleStation
			});

			db.Routes.Add(new Route
			{
				StartStation = EndStation,
				EndStation = StartStation,
				MiddleStation = MiddleStation
			});
		}
	}
}
