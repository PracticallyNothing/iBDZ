using iBDZ.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iBDZ.App.Controllers
{
	public class AdminController : Controller
    {
		private readonly IAdminService adminService;
		private readonly IUserService userService;

		public AdminController(IAdminService adminService, IUserService userService)
		{
			this.adminService = adminService;
			this.userService = userService;
		}

		[HttpGet]
		[Authorize(Roles = "Administrator, SuperUser")]
        public IActionResult Users()
        {
            return View(adminService.GetAllUsers());
        }

		[HttpGet]
		[Authorize(Roles = "Administrator, SuperUser")]
		public IActionResult UserInfo(string id)
		{
			return View(adminService.GetUserInfo(id));
		}

		[HttpGet]
		[Authorize(Roles = "Administrator, SuperUser")]
		public IActionResult ViewReceipt(string username, string id)
		{
			return View("~/Views/User/Receipt.cshtml", userService.GetReceipt(username, id));
		}
	}
}