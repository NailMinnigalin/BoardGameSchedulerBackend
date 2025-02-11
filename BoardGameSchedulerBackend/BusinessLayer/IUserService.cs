using BoardGameSchedulerBackend.DataLayer;

namespace BoardGameSchedulerBackend.BusinessLayer
{
	public interface IUserService
	{
		Task RegisterUserAsync(string userName, string email, string password);
		Task<User?> GetUserAsync(Guid id);
	}
}
