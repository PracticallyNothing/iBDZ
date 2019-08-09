using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using System.Threading;

namespace iBDZ.App
{
	public class Program
    {
        public static void Main(string[] args)
        {
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
			CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
