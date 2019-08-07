using iBDZ.Data.ViewModels;
using System.Collections.Generic;

namespace iBDZ.Services
{
	public interface ISeatService
	{
		List<SeatSearchResultModel> FindSeats(string jsonParams);
	}
}