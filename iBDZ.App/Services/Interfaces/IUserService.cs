﻿using iBDZ.Data.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;

namespace iBDZ.App.Services
{
	public interface IUserService
    {
		List<ShortPurchaseModel> GetUserPurchasesList(ClaimsPrincipal user);
		ReceiptModel GetReceipt(ClaimsPrincipal user, string rid);
	}
}