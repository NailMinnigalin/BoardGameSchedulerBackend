using Microsoft.AspNetCore.Mvc.Filters;

namespace BoardGameSchedulerBackend.InfrastructureLayer
{
	public class CleanDbLockFilter : IAsyncActionFilter
	{
		private static readonly ManualResetEventSlim _dbResetEvent = new(true);
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var path = context.HttpContext.Request.Path.Value ?? "";
			var isCleanDb = path.EndsWith("cleandb", StringComparison.OrdinalIgnoreCase);

			if (isCleanDb)
			{
				_dbResetEvent.Reset();

				try
				{
					await next();
				}
				finally
				{
					_dbResetEvent.Set();
				}
			}
			else
			{
				await Task.Run(() => _dbResetEvent.Wait());
				await next();
			}
		}
	}
}
