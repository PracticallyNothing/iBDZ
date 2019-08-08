using iBDZ.Data.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;

namespace iBDZ.Services
{
	public interface ISeatService
	{
		List<SeatSearchResultModel> FindSeats(string jsonParams);
		string ReserveSeat(ClaimsPrincipal user, string seatId);
		ReservationInfoModel GetReservationInfo(string car, string coupe);
	}
}