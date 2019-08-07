using iBDZ.Data.ViewModels;
using iBDZ.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

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

		[HttpPost]
		[Authorize(Roles = "Administrator, SuperUser")]
		public IActionResult Delete(string id)
		{
			trainService.DeleteTrain(id);
			return Redirect("/Train/Timetable");
		}

		[HttpGet]
		[Authorize(Roles = "Administrator, SuperUser")]
		public IActionResult New()
		{
			string id = trainService.GenerateNewTrain();
			return Redirect("/Train/Edit?id="+id);
		}

		[HttpPost]
		[Authorize(Roles = "Administrator, SuperUser")]
		public IActionResult Edit()
		{
			trainService.EditTrain(new StreamReader(Request.Body).ReadToEnd());
			return Redirect("/Train/Timetable");
		}

		[HttpGet]
		[Authorize(Roles = "Administrator, SuperUser")]
		public IActionResult Edit(string id)
		{
			return View(trainService.GetTrainInfoFromId(id));
		}
	}
}
