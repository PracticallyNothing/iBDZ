using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace iBDZ.Data
{
	public class User : IdentityUser
	{
		public User() {}

		public List<Purchase> Purchases { get; set; }
	}
}