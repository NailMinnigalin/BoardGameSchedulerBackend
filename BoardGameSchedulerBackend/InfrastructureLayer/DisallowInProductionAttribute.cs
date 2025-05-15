using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BoardGameSchedulerBackend.InfrastructureLayer
{
	public class DisallowInProductionAttribute : ActionFilterAttribute
	{
		private readonly IWebHostEnvironment _env;

		public DisallowInProductionAttribute(IWebHostEnvironment env)
		{
			_env = env;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (_env.IsProduction())
			{
				context.Result = new StatusCodeResult(StatusCodes.Status404NotFound);
			}
		}
	}
}
