using iBDZ.Data;
using iBDZ.Data.ViewModels;
using System;
using System.Collections.Generic;

namespace iBDZ.Services
{
	public interface ITrainService
	{
		TrainInfoModel GetTrainInfoFromId(string trainId);
		Tuple<List<ShortTrainInfoModel>, string, string> GetTimetable(string startStation, string endStation);
		void EditTrain(string json);
		void DeleteTrain(string id);
		string GenerateNewTrain();
	}
}