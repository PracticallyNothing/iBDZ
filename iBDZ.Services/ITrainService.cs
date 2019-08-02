using iBDZ.Data.ViewModels;

namespace iBDZ.Services
{
	public interface ITrainService
	{
		TrainInfoModel GetTrainFromId(string trainId);
	}
}