using BoardGameSchedulerBackend.DataLayer;

namespace BoardGameSchedulerBackend.BusinessLayer
{
	public interface IUserService
	{
		Task<UserCreationResult> RegisterUserAsync(string userName, string email, string password);
		Task<User?> GetUserAsync(Guid id);
		Task<SignInResult> SignInAsync(string userName, string password);
		Task SignOut();
	}
}
