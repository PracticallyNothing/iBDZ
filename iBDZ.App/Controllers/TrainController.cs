using iBDZ.Services;
using Microsoft.AspNetCore.Mvc;

namespace iBDZ.App.Controllers
{
	public class TrainController : Controller
	{
		private readonly ITrainService trainService;

		public TrainController(ITrainService trainService)
		{
			this.trainService = trainService;
		}

		[HttpGet]
		public IActionResult Timetable(string ss, string es)
		{
			return View(trainService.GetTimetable(ss, es));
		}

		[HttpGet]
		public IActionResult Info(string id)
		{
			return View(trainService.GetTrainInfoFromId(id));
		}
	}
}
