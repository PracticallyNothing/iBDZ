using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iBDZ.App.Data;
using iBDZ.Data;
using iBDZ.Data.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace iBDZ.Services
{
	public class AdminService : IAdminService
	{
		private readonly iBDZDbContext db;
		private readonly UserManager<User> userManager;
		private readonly IUserService userService;

		public AdminService(iBDZDbContext db, UserManager<User> userManager, IUserService userService)
		{
			this.db = db;
			this.userManager = userManager;
			this.userService = userService;
		}

		private int GetRoleOrder(string role)
		{
			List<string> roleOrder = new List<string>(3) {
				"User",
				"Administrator",
				"SuperUser"
			};

			return roleOrder.FindIndex(x => x == role);
		}

		private List<string> GetRolesForUser(string userId)
		{
			List<string> roleIds = db.UserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToList();
			return db.Roles
				.Where(x => roleIds.Contains(x.Id))
				.Select(x => x.Name)
				.OrderBy(x => GetRoleOrder(x))
				.ToList();
		}

		public List<ShortUserInfo> GetAllUsers()
		{
			List<ShortUserInfo> userInfo = new List<ShortUserInfo>();

			foreach (var user in db.Users.Include(x => x.Receipts).ToList())
			{
				userInfo.Add(new ShortUserInfo
				{
					Id = user.Id,
					UserName = user.UserName,
					Roles = GetRolesForUser(user.Id),
					LastPurchase = (
						user.Receipts.Count == 0
						? (DateTime?)null
						: user.Receipts
							.Select(y => y.TimeOfPurchase)
							.OrderBy(y => y)
							.First()
					)
				});
			}

			return userInfo;
		}

		public UserInfo GetUserInfo(string userId)
		{
			User user = db.Users.Include(x => x.Receipts).Where(x => x.Id == userId).FirstOrDefault();
			if (user == null)
				return new UserInfo();

			return new UserInfo()
			{
				Id = user.Id,
				UserName = user.UserName,
				Roles = GetRolesForUser(user.Id),
				Purchases = userService.GetUserPurchasesList(user.UserName)
			};
		}

		public void PromoteUser(string userId)
		{
			User u = db.Users.Find(userId);
			if (u == null)
			{
				return;
			}

			var isSuperUserTask = userManager.IsInRoleAsync(u, "SuperUser");
			if (isSuperUserTask.Result)
			{
				return;
			}

			var isAdminTask = userManager.IsInRoleAsync(u, "Administrator");
			if (!isAdminTask.Result)
			{
				var addRoleTask = userManager.AddToRoleAsync(u, "Administrator");
				addRoleTask.Wait();
			}
		}

		public void DemoteUser(string userId)
		{
			User u = db.Users.Find(userId);
			if (u == null)
			{
				return;
			}

			var isSuperUserTask = userManager.IsInRoleAsync(u, "SuperUser");
			if (isSuperUserTask.Result)
			{
				return;
			}

			var isAdminTask = userManager.IsInRoleAsync(u, "Administrator");
			if (isAdminTask.Result)
			{
				var removeRoleTask = userManager.RemoveFromRoleAsync(u, "Administrator");
				removeRoleTask.Wait();
			}
		}
	}
}
