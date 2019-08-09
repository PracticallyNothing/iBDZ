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
            return View(userService.GetReceipt(User, id));
        }

		[HttpGet]
		[Authorize]
		public IActionResult Purchases()
		{
			return View(userService.GetUserPurchasesList(User));
		}
    }
}