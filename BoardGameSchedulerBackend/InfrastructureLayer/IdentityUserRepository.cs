using BoardGameSchedulerBackend.BusinessLayer.ApplicationLayer;
using BoardGameSchedulerBackend.DataLayer;
using Microsoft.AspNetCore.Identity;

namespace BoardGameSchedulerBackend.Infrastructure
{
    public class IdentityUserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityUserRepository(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task CreateAsync(User user, string password)
        {
            var identityUser = new IdentityUser
            {
                UserName = user.UserName,
                Email = user.Email,
            };

            var result = await _userManager.CreateAsync(identityUser, password);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            user.Id = Guid.Parse(identityUser.Id);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var identityUser = await _userManager.FindByIdAsync(id.ToString());
            if (identityUser == null)
                return null;

            return new User
            {
                Id = id,
                UserName = identityUser.UserName,
                Email = identityUser.Email
            };
        }
    }
}
