using iBDZ.App.Data;
using iBDZ.Data.ViewModels;
using System;

namespace iBDZ.Services
{
	public class TrainService : ITrainService
	{
		private readonly ApplicationDbContext db;

		public TrainService(ApplicationDbContext db)
		{
			this.db = db;
		}

		public TrainInfoModel GetTrainFromId(string trainId)
		{
			return new TrainInfoModel{};
		}
	}
}
