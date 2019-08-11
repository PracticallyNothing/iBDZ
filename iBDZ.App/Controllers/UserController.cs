using iBDZ.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iBDZ.App.Controllers
{
	public class UserController : Controller
    {
		private IUserService userService;

		public UserController(IUserService userService)
		{
			this.userService = userService;
		}
		
		[HttpGet]
		[Authorize]
        public IActionResult Receipt(string id)
        {
			var receipt = userService.GetReceipt(User.Identity.Name, id);

			if (receipt.Id == "")
				return Redirect("/User/Purchases");
			else
				return View(receipt);
        }

		[HttpPost]
		[Authorize]
		public IActionResult Refund(string id)
		{
			userService.RefundPurchase(User, id);
			return Redirect("/User/Purchases");
		}

		[HttpGet]
		[Authorize]
		public IActionResult Purchases()
		{
			return View(userService.GetUserPurchasesList(User.Identity.Name));
		}
    }
}