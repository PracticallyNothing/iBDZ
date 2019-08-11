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
			AddRoutes(serviceProvider);
		}

		public static void AddRoutes(IServiceProvider serviceProvider)
		{
			var db = serviceProvider.GetRequiredService<iBDZDbContext>();
			if (!db.Routes.Any())
			{
				AddRoute(db, "Варна", "Горна Оряховица", "София");
				AddRoute(db, "Варна", "Карлово", "София");

				AddRoute(db, "Варна", "Русе");
				AddRoute(db, "София", "Русе");

				AddRoute(db, "Русе", "Пловдив");

				AddRoute(db, "Видин", "София");
				AddRoute(db, "Берковица", "Видин");
				AddRoute(db, "Берковица", "София");

				AddRoute(db, "Истанбул", "София");
				AddRoute(db, "Пловдив", "София");
				AddRoute(db, "Пловдив", "Истанбул");

				AddRoute(db, "Солун", "София");
				AddRoute(db, "Благоевград", "София");
				AddRoute(db, "Благоевград", "Солун");
			}
			db.SaveChanges();
		}

		public static void AddRoute(iBDZDbContext db, string StartStation, string EndStation)
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

		public static void AddRoute(iBDZDbContext db, string StartStation, string MiddleStation, string EndStation)
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
