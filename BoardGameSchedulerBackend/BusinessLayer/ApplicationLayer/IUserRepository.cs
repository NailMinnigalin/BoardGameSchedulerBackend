using BoardGameSchedulerBackend.DataLayer;

namespace BoardGameSchedulerBackend.BusinessLayer.ApplicationLayer
{
	public interface IUserRepository
	{
		Task<User?> GetByIdAsync(Guid id);
		Task CreateAsync(string userName, string userEmail, string password);
	}
}
