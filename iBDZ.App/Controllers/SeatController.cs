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
		[ActionName("Reserve")]
		public IActionResult ReservePost()
		{
			string receiptId = seatService.ReserveSeat(User, new StreamReader(Request.Body).ReadToEnd());
			if (receiptId == "")
			{
				return Redirect("/Seat/Find");
			}
			else
			{
				return Redirect("/User/Receipt?id=" + receiptId);
			}
		}

		[HttpGet]
		[Authorize]
		public IActionResult Reserve(string car, string coupe)
		{
			return View(seatService.GetReservationInfo(car, coupe));
		}
	}
}