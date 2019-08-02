using iBDZ.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
		public IActionResult Timetable() {
			return View();
		}

		[HttpGet]
		public IActionResult Info(string trainId)
		{
			return View(trainService.GetTrainFromId(trainId));
		}
	}
}
