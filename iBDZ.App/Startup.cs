using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iBDZ.App.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using iBDZ.Services;
using iBDZ.App.Data.Seeders;
using iBDZ.Data;
using System.Collections.Generic;
using System.Linq;
using iBDZ.App.Helpers;

namespace iBDZ.App
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddScoped<IRouteService, RouteService>();
			services.AddScoped<ITrainService, TrainService>();
			services.AddScoped<ISeatService, SeatService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IAdminService, AdminService>();

			services.AddDbContext<iBDZDbContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
			});
			services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<iBDZDbContext>();

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseMvcWithDefaultRoute();

			TrainSeeder trainSeeder = new TrainSeeder(); trainSeeder.Seed(serviceProvider);

			RouteSeeder.AddRoutes(serviceProvider);
			TrainSeeder.AddTrains(serviceProvider);
			RolesSeeder.AddRoles(serviceProvider);
			RolesSeeder.AddSuperUser(serviceProvider);

			TrainManager tm = new TrainManager(Configuration); tm.Start();
		}
	}
}
