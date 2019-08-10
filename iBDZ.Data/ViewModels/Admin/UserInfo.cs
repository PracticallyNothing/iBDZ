using System;
using System.Collections.Generic;
using System.Text;

namespace iBDZ.Data.ViewModels
{
	public class UserInfo
	{	
		public UserInfo() {
			UserName = "Mr. Doesn't Existov";
			Roles = new List<string>() { "Check the Id" };
			Purchases = new List<ShortReceiptModel>();
		}

		public string Id { get; set; }

		public string UserName { get; set; }

		public List<string> Roles { get; set; }

		public List<ShortReceiptModel> Purchases {get;set;}
    }
}
