namespace BoardGameSchedulerBackend.BusinessLayer
{
	public interface IDbManager
	{
		Task<bool> CleanDb();
	}
}
