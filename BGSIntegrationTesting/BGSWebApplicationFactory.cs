using BoardGameSchedulerBackend.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;

namespace BGSIntegrationTesting
{
	public class BGSWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				// Remove any existing registrations for ApplicationDbContext and related options.
				services.RemoveAll<ApplicationDbContext>();
				services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
				services.RemoveAll<DbConnection>();

				// Create a SQLite in-memory database with shared cache.
				var sqliteConnection = new SqliteConnection("DataSource=:memory:");
				sqliteConnection.Open();
				services.AddSingleton<DbConnection>(sqliteConnection);

				// Re-register ApplicationDbContext to use our SQLite connection.
				services.AddDbContext<ApplicationDbContext>((container, options) =>
				{
					var connection = container.GetRequiredService<DbConnection>();
					options.UseSqlite(connection);
				});

				services.AddIdentity<IdentityUser, IdentityRole>()
					.AddEntityFrameworkStores<ApplicationDbContext>()
					.AddDefaultTokenProviders();

				// Build the service provider so we can run the migration on the same connection.
				var sp = services.BuildServiceProvider();
				using (var scope = sp.CreateScope())
				{
					var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
					// Use EnsureCreated() if you don't want to run migrations, or Migrate() if you do.
					dbContext.Database.Migrate();
				}
			});

			base.ConfigureWebHost(builder);
		}
	}
}
