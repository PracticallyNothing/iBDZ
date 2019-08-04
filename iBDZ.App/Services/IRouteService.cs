﻿using iBDZ.Data;
using System.Collections.Generic;

namespace iBDZ.Services
{
	public interface IRouteService
	{
		string GetShortRouteStringFromId(string routeId);
		List<string> GetAllStartingStations();
		List<string> GetAllEndStations();
		Route GetRouteFromId(string routeId);
	}
}