using System;

namespace iBDZ.App.Data.Seeders
{
	public interface ISeeder
	{
		void Seed(IServiceProvider serviceProvider);
	}
}