using iBDZ.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iBDZ.App.Controllers
{
	public class AdminController : Controller
    {
		private readonly IAdminService adminService;

		public AdminController(IAdminService adminService)
		{
			this.adminService = adminService;
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
	}
}