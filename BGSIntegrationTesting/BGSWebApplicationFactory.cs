using BoardGameSchedulerBackend.Infrastructure;
using BoardGameSchedulerBackend.InfrastructureLayer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace BGSIntegrationTesting
{
	public class BGSWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("Test");

			builder.ConfigureServices(services =>
			{
				CreateSQLiteDb(services);
				RegisterApplicationDbContext(services);
				services.AddCustomIdentity();

				RunMigration(services);
			});

			base.ConfigureWebHost(builder);
		}

		private static void RunMigration(IServiceCollection services)
		{
			var sp = services.BuildServiceProvider();
			using (var scope = sp.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				dbContext.Database.Migrate();
			}
		}

		private static void RegisterApplicationDbContext(IServiceCollection services)
		{
			services.AddDbContext<ApplicationDbContext>((container, options) =>
			{
				var connection = container.GetRequiredService<DbConnection>();
				options.UseSqlite(connection);
			});
		}

		private static void CreateSQLiteDb(IServiceCollection services)
		{
			var sqliteConnection = new SqliteConnection("DataSource=:memory:");
			sqliteConnection.Open();
			services.AddSingleton<DbConnection>(sqliteConnection);
		}
	}
}
