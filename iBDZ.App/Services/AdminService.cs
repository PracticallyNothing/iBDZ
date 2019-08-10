using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iBDZ.App.Data;
using iBDZ.Data;
using iBDZ.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace iBDZ.Services
{
	public class AdminService : IAdminService
	{
		private readonly iBDZDbContext db;
		private readonly IUserService userService;

		public AdminService(iBDZDbContext db, IUserService userService)
		{
			this.db = db;
			this.userService = userService;
		}

		private List<string> GetRolesForUser(string userId)
		{
			List<string> roleIds = db.UserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToList();
			return db.Roles.Where(x => roleIds.Contains(x.Id)).Select(x => x.Name).ToList();
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
					LastPurchase =
					user.Receipts
						.Select(y => y.TimeOfPurchase)
						.OrderBy(y => y)
						.FirstOrDefault()
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
	}
}
