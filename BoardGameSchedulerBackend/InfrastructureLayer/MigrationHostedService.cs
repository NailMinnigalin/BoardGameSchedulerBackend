using BoardGameSchedulerBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BoardGameSchedulerBackend.InfrastructureLayer
{
	public static partial class MigrationHostedServiceLogger
	{
		[LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Starting database migration...")]
		public static partial void LogStartingDatabaseMigration(this ILogger logger);

		[LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Database migration completed.")]
		public static partial void LogDatabaseMigrationCompleted(this ILogger logger);
	}

	public class MigrationHostedService : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _environment;
		private readonly ILogger<MigrationHostedService> _logger;

		public MigrationHostedService(IServiceProvider serviceProvider, IConfiguration configuration, IWebHostEnvironment environment, ILogger<MigrationHostedService> logger)
		{
			_serviceProvider = serviceProvider;
			_configuration = configuration;
			_environment = environment;
			_logger = logger;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			if (!_environment.IsDevelopment() ||  !_configuration.GetValue<bool>("ApplyMigarionOnStartup"))
			{
				return;
			}

			using var scope = _serviceProvider.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			if (await dbContext.Database.CanConnectAsync(cancellationToken))
			{
				var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
				if (pendingMigrations.Any())
				{
					_logger.LogStartingDatabaseMigration();
					await dbContext.Database.MigrateAsync(cancellationToken);
					_logger.LogDatabaseMigrationCompleted();
				}
			}
			else
			{
				throw new DataException("Cannot connect to the database to apply migrations.");
			}
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}
