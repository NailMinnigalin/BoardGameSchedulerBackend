using BoardGameSchedulerBackend.BusinessLayer;
using BoardGameSchedulerBackend.Infrastructure;

namespace BoardGameSchedulerBackend.InfrastructureLayer
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
		public async Task<bool> CleanDb()
		{
			var deleteResult = await _applicationDbContext.Database.EnsureDeletedAsync();
			if (!deleteResult) return deleteResult;

			var createResult = await _applicationDbContext.Database.EnsureCreatedAsync();
			return createResult;
		}
	}
}
