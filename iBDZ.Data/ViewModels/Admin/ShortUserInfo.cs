using System;
using System.Collections.Generic;

namespace iBDZ.Data.ViewModels
{
	public class ShortUserInfo
    {
		public string Id { get; set; }
		
		public string UserName { get; set; }

		public List<string> Roles { get; set; }

		public DateTime LastPurchase { get; set; }
    }
}
