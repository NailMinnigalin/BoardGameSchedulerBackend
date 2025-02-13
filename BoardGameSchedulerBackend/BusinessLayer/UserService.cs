using BoardGameSchedulerBackend.DataLayer;

namespace BoardGameSchedulerBackend.BusinessLayer
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		public UserService(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<UserCreationResult> RegisterUserAsync(string userName, string email, string password)
		{
			return await _userRepository.CreateAsync(userName, email, password);
		}

		public async Task<User?> GetUserAsync(Guid id)
		{
			return await _userRepository.GetByIdAsync(id);
		}

		public async Task<SignInResult> SignInAsync(string userName, string password)
		{
			return await _userRepository.SignInAsync(userName, password);
		}
	}
}
