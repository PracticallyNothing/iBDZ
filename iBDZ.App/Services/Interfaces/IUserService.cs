using iBDZ.Data.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;

namespace iBDZ.Services
{
	public interface IUserService
    {
		List<ShortReceiptModel> GetUserPurchasesList(string username);
		ReceiptModel GetReceipt(string username, string rid);
	}
}
