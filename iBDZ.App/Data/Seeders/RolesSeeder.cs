using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iBDZ.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace iBDZ.App.Data.Seeders
{
	public class RolesSeeder : ISeeder
	{
		private const string SuperUserEmail = "SuperUser@sudo.com";
		private const string SuperUserPassword = "SuperUserPassword_1";

		public void ClearRoles(IServiceProvider serviceProvider)
		{
			var db = serviceProvider.GetRequiredService<iBDZDbContext>();
			foreach (var r in db.Roles)
				db.Roles.Remove(r);
			db.SaveChanges();
		}

		public void ClearSuperUser(IServiceProvider serviceProvider)
		{
			var db = serviceProvider.GetRequiredService<iBDZDbContext>();
			var superUser = db.Users.FirstOrDefault(x => x.UserName == SuperUserEmail);
			if (superUser != null)
			{
				db.Users.Remove(superUser);
				db.SaveChanges();
			}
		}

		public void Seed(IServiceProvider serviceProvider)
		{
			AddRoles(serviceProvider);
			AddSuperUser(serviceProvider);
		}

		public static void AddSuperUser(IServiceProvider serviceProvider)
		{
			var db = serviceProvider.GetRequiredService<iBDZDbContext>();
			UserManager<User> userManager = serviceProvider.GetRequiredService<UserManager<User>>();

			User u = new User { UserName = SuperUserEmail, Email = SuperUserEmail };
			User userSearch = db.Users.FirstOrDefault(x => x.UserName == u.UserName);

			if (userSearch == null)
			{
				try
				{
					var res = userManager.CreateAsync(u, SuperUserPassword);
					res.Wait();
					db.Users.Add(u);
				} catch { }
			}
			else
			{
				u = userSearch;
			}

			List<IdentityRole> roles = db.Roles.ToList();
			foreach (var r in roles)
			{
				if (!db.UserRoles.Any(x => x.UserId == u.Id && x.RoleId == r.Id))
					db.UserRoles.Add(new IdentityUserRole<string>
					{
						RoleId = r.Id,
						UserId = u.Id
					});
			}

			//List<string> roles = db.Roles.Select(x => x.Name).ToList();
			//var res2 = userManager.AddToRolesAsync(u, roles);
			//res2.Wait();

			db.SaveChanges();
		}

		public static void AddRoles(IServiceProvider serviceProvider)
		{
			var db = serviceProvider.GetRequiredService<iBDZDbContext>();

			if (!db.Roles.Any())
			{
				db.Roles.AddRange(
					new IdentityRole { Name = "User", NormalizedName = "USER" },
					new IdentityRole { Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
					new IdentityRole { Name = "SuperUser", NormalizedName = "SUPERUSER" }
				);
				db.SaveChanges();
			}
		}
	}
}