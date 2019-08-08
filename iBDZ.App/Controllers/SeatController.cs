using iBDZ.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace iBDZ.App.Controllers
{
	public class SeatController : Controller
    {
		private readonly ISeatService seatService;

		public SeatController(ISeatService seatService)
		{
			this.seatService = seatService;
		}

		[HttpPost]
		[Authorize]
		[ActionName("Find")]
		public IActionResult FindPost()
		{
			return Json(seatService.FindSeats(new StreamReader(Request.Body).ReadToEnd()));
		}

		[HttpGet]
		[Authorize]
		public IActionResult Find()
        {
            return View();
        }

		[HttpPost]
		[Authorize]
		public IActionResult ReservePost()
		{
			return View(seatService.ReserveSeat(User, new StreamReader(Request.Body).ReadToEnd()));
		}

		[HttpGet]
		[Authorize]
		public IActionResult Reserve(string car, string coupe)
		{
			return View(seatService.GetReservationInfo(car, coupe));
		}
    }
}