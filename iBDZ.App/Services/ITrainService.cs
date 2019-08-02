using iBDZ.Data.ViewModels;
using System.Collections.Generic;

namespace iBDZ.Services
{
	public interface ITrainService
	{
		TrainInfoModel GetTrainFromId(string trainId);
		List<ShortTrainInfoModel> GetTimetable();
	}
}