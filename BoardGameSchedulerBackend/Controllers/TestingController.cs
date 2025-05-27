using BoardGameSchedulerBackend.BusinessLayer;
using BoardGameSchedulerBackend.InfrastructureLayer;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameSchedulerBackend.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[TypeFilter(typeof(DisallowInProductionAttribute))]
	public class TestingController : ControllerBase
	{
		private readonly IDbManager _dbManager;

		public TestingController(IDbManager dbManager)
		{
			_dbManager = dbManager;
		}

		[HttpDelete]
		[Route("cleandb")]
		public async Task<IActionResult> CleanDb()
		{
			await _dbManager.CleanDb();
			return Ok();
		}
	}
}
