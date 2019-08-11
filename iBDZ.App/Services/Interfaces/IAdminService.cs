using iBDZ.Data.ViewModels;
using System.Collections.Generic;

namespace iBDZ.Services
{
	public interface IAdminService
    {
		List<ShortUserInfo> GetAllUsers();
		UserInfo GetUserInfo(string userId);
		void PromoteUser(string userId);
		void DemoteUser(string userId);
	}
}
