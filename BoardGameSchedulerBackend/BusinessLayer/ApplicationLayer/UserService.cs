using BoardGameSchedulerBackend.DataLayer;

namespace BoardGameSchedulerBackend.BusinessLayer.ApplicationLayer
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task RegisterUserAsync(string userName, string email, string password)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Email = email
            };

            await _userRepository.CreateAsync(user, password);
        }

        public async Task<User?> GetUserAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }
    }
}
