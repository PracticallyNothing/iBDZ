using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBDZ.App.Data.Seeders
{
	public class RolesSeeder : ISeeder
	{
		public void Seed(IServiceProvider serviceProvider)
		{
			var db = serviceProvider.GetRequiredService<ApplicationDbContext>();
			if (!db.Roles.Any())
			{
				db.Roles.AddRange(
					new IdentityRole("User"),
					new IdentityRole("Administrator"),
					new IdentityRole("SuperUser")
				);
				db.SaveChanges();
			}
		}
	}
}
