using BoardGameSchedulerBackend.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace BGSIntegrationTesting
{
	public class BGSWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				RemoveDbContextService(services);
				RemoveDbConnectionService(services);
				ConfigureInMemorySqliteDb(services);
			});

			base.ConfigureWebHost(builder);
		}

		private static void ConfigureInMemorySqliteDb(IServiceCollection services)
		{
			// Add ApplicationDbContext with SQLite In-Memory Database
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlite("DataSource=:memory:");
			});

			// Ensure the database is created and initialized
			var sp = services.BuildServiceProvider();
			using var scope = sp.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			dbContext.Database.OpenConnection(); // Required for SQLite in-memory
			dbContext.Database.EnsureCreated();
		}

		private static void RemoveDbConnectionService(IServiceCollection services)
		{
			var dbConnectionDescriptor = services.SingleOrDefault(
								d => d.ServiceType ==
									typeof(DbConnection));

			services.Remove(dbConnectionDescriptor);
		}

		private static void RemoveDbContextService(IServiceCollection services)
		{
			var dbContextDescriptor = services.SingleOrDefault(
								d => d.ServiceType ==
									typeof(DbContextOptions<ApplicationDbContext>));

			services.Remove(dbContextDescriptor);
		}
	}
}
