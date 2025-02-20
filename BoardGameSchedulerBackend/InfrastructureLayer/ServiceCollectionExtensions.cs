using BoardGameSchedulerBackend.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace BoardGameSchedulerBackend.InfrastructureLayer
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
		{
			services.AddIdentity<IdentityUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.Configure<IdentityOptions>(options =>
			{
				// Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 1;

				// Lockout settings.
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;

				// User settings.
				options.User.AllowedUserNameCharacters =
				"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
				options.User.RequireUniqueEmail = false;
			});

			services.ConfigureApplicationCookie(options =>
			{
				// Common cookie settings
				options.Cookie.HttpOnly = true;
				options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
				options.LoginPath = "/login";
				options.AccessDeniedPath = "/accessdenied";
				options.SlidingExpiration = true;

				// Override redirection to return status codes instead of redirects
				options.Events.OnRedirectToLogin = context =>
				{
					context.Response.StatusCode = StatusCodes.Status401Unauthorized;
					return Task.CompletedTask;
				};

				options.Events.OnRedirectToAccessDenied = context =>
				{
					context.Response.StatusCode = StatusCodes.Status403Forbidden;
					return Task.CompletedTask;
				};
			});

			return services;
		}
	}
}
