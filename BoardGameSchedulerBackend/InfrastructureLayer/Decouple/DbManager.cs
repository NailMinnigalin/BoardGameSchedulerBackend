using BoardGameSchedulerBackend.BusinessLayer;
using BoardGameSchedulerBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BoardGameSchedulerBackend.InfrastructureLayer.Decouple
{
	public class DbManager : IDbManager
	{
		private readonly ApplicationDbContext _applicationDbContext;

		public DbManager(ApplicationDbContext applicationDbContext)
		{
			_applicationDbContext = applicationDbContext;
		}

		/// <summary>
		/// Delete and recreate whole db
		/// </summary>
		/// <returns>true - if recreation was sucessful, false - if not</returns>
		public async Task CleanDb()
		{
			await _applicationDbContext.Database.EnsureDeletedAsync();
			await _applicationDbContext.Database.MigrateAsync();
		}
	}
}
